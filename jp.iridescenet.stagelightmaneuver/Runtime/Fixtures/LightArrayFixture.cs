using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if USE_HDRP

using UnityEngine.Rendering.HighDefinition;
#elif USE_URP

using UnityEngine.Rendering.Universal;

#endif


#if USE_VLB
using VLB;
#endif

namespace StageLightManeuver
{
    public class LightArrayFixture:StageLightFixtureBase
    {
        [SerializeField] private List<LightFixture> lightFixtures;
    
    
    public override void Init()
        {
            base.Init();

            foreach (var lightFixture in lightFixtures)
            {
                lightFixture.Init();
            }
            
            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(LightProperty));

        }

        public override void EvaluateQue(float currentTime)
        {
            if(lightFixtures == null) return;
            base.EvaluateQue(currentTime);

            while (stageLightDataQueue.Count>0)
            {
                var data = stageLightDataQueue.Dequeue();
                var stageLightBaseProperty= data.TryGet<ClockProperty>() as ClockProperty;
                var lightProperty = data.TryGet<LightProperty>() as LightProperty;
                var lightColorProperty = data.TryGet<LightColorProperty>() as LightColorProperty;
                var lightIntensityProperty = data.TryGet<LightIntensityProperty>() as LightIntensityProperty;
                var weight = data.weight;
                if(lightProperty == null || stageLightBaseProperty == null) continue;
             
                // Debug.Log($"{lightProperty.clockOverride.value.childStagger}, {lightProperty.clockOverride.value.propertyOverride}");
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightProperty),Index);
                var manualLightArrayProperty = data.TryGet<ManualLightArrayProperty>();
                var manualColorArrayProperty = data.TryGet<ManualColorArrayProperty>();
                
                

                if (manualLightArrayProperty != null)
                {
                    var values = manualLightArrayProperty.lightValues.value;
                    if (Index < values.Count)
                    {
                        var lightValue = values[Index];
                        lightIntensity += lightValue.intensity * weight;
                        spotAngle += lightValue.angle * weight;
                        innerSpotAngle += lightValue.innerAngle * weight;
                        spotRange += lightValue.range * weight;
                    }
                }
                else
                {
                    if (lightIntensityProperty != null)
                    {
                        var t =lightIntensityProperty.clockOverride.propertyOverride ? SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightIntensityProperty),Index) : normalizedTime;
                        lightIntensity += lightIntensityProperty.lightToggleIntensity.value.Evaluate(t) * weight;
                    }
                    spotAngle += lightProperty.spotAngle.value.Evaluate(normalizedTime) * weight;
                    innerSpotAngle += lightProperty.innerSpotAngle.value.Evaluate(normalizedTime) * weight;
                    spotRange += lightProperty.range.value.Evaluate(normalizedTime) * weight;
                }

                if (manualColorArrayProperty != null)
                {
                    var values = manualColorArrayProperty.colorValues.value;
                    if (Index < values.Count)
                    {
                        var colorValue = values[Index];
                        lightColor += colorValue.color * weight;
                    }
                    
                }else if (lightColorProperty != null)
                {
                    var t =lightColorProperty.clockOverride.propertyOverride ? SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightColorProperty),Index) : normalizedTime;
                    lightColor += lightColorProperty.lightToggleColor.value.Evaluate(t) * weight;
                }

                if (weight > 0.5f)
                {
                    lightCookie = lightProperty.cookie.value;
#if USE_VLB
                    if(volumetricCookieHd)volumetricCookieHd.cookieTexture = lightProperty.cookie.value;
#endif
                }
            }
            
            lightIntensity = Mathf.Clamp(lightIntensity, limitIntensityMin, limitIntensityMax);
            spotAngle = Mathf.Clamp(spotAngle, limitSpotAngleMin, limitSpotAngleMax);
            innerSpotAngle = Mathf.Clamp(innerSpotAngle, limitInnerSpotAngleMin, limitInnerSpotAngleMax);
            spotRange = Mathf.Clamp(spotRange, limitSpotRangeMin, limitSpotRangeMax);
            
            
        }

        public override void UpdateFixture()
        {
            if (lights==null) return;
            foreach (var light in lights)
            {
#if USE_HDRP
                if (lightData.ContainsKey(light))
                {
                    var hdAdditionalLightData = lightData[light];
                    // Debug.Log(hdAdditionalLightData.intensity);
                    // hdAdditionalLightData.SetIntensity(lightIntensity);
                    // hdAdditionalLightData.SetLightDimmer(lightIntensity);
                    hdAdditionalLightData.intensity = lightIntensity;
                    hdAdditionalLightData.color = lightColor;
                    hdAdditionalLightData.SetSpotAngle(spotAngle);
                    hdAdditionalLightData.innerSpotPercent = innerSpotAngle;
                    hdAdditionalLightData.range = spotRange;
                    if(lightCookie)hdAdditionalLightData.SetCookie(lightCookie);
                    // hdAdditionalLightData.UpdateAllLightValues();
                    // hdAdditionalLightData.setli
                    // lightData[light].intensity=lightIntensity;
                }
#else
                light.color = lightColor;
                 light.intensity = lightIntensity;
                light.spotAngle = spotAngle;
                light.innerSpotAngle = innerSpotAngle;
                light.range = spotRange;
                light.cookie = lightCookie;
#endif

#if USE_VLB
                if(volumetricCookieHd) volumetricCookieHd.cookieTexture = lightCookie;
#endif
            }
            
            
          
        }
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
    }
}