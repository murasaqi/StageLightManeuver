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
    [ExecuteAlways]
    // [AddComponentMenu("")]
    public class LightFixture : StageLightFixtureBase
    {
        public List<Light> lights = new List<Light>();
#if USE_HDRP
        public Dictionary<Light,HDAdditionalLightData> lightData = new Dictionary<Light, HDAdditionalLightData>();
#endif
        public Color lightColor;
        public float lightIntensity;
        public float spotAngle;
        public float innerSpotAngle;
        public float spotRange;
        public Texture lightCookie;
        public float limitIntensityMin = 0f;
        public float limitIntensityMax = 10000f;
        public float limitSpotAngleMin = 0f;
        public float limitSpotAngleMax = 100f;
        public float limitInnerSpotAngleMin = 0f;
        public float limitInnerSpotAngleMax = 100f;
        public float limitSpotRangeMin = 0f;
        public float limitSpotRangeMax = 100f;
#if USE_VLB
        public VolumetricLightBeamHD volumetricLightBeamHd;
        public VolumetricCookieHD volumetricCookieHd;
#endif
        // public UniversalAdditionalLightData universalAdditionalLightData;


        public void GetLightInChildrenAndFetchData()
        {
            var lightList = GetComponentsInChildren<Light>().ToList();
            
            if(lightList.Count <= 0) return;
            lightColor = lightList[0].color;
            lightIntensity = lightList[0].intensity;
            spotAngle = lightList[0].spotAngle;
            innerSpotAngle = lightList[0].innerSpotAngle;
            spotRange = lightList[0].range;
            lightCookie = lightList[0].cookie;
            
        }
        public override void Init()
        {
            base.Init();
#if USE_HDRP
            lightData.Clear();
#endif
            foreach (var light in lights)
            {
                if(light == null) continue;
                lightColor = light.color;
                lightIntensity = light.intensity;
                spotAngle = light.spotAngle;
                innerSpotAngle = light.innerSpotAngle;
                spotRange = light.range;
                lightCookie = light.cookie;
#if USE_HDRP
                lightData.Add(light, light.GetComponent<HDAdditionalLightData>());
#endif

#if USE_VLB
                volumetricLightBeamHd = light.GetComponent<VolumetricLightBeamHD>();
                volumetricCookieHd = light.GetComponent<VolumetricCookieHD>();
#endif
                // PropertyType.Add(typeof(property));
            }
            PropertyTypes = new List<Type>();
            PropertyTypes.Add(typeof(LightProperty));
            PropertyTypes.Add(typeof(LightColorProperty));
            PropertyTypes.Add(typeof(LightIntensityProperty));

        }

        public override void EvaluateQue(float currentTime)
        {
            if(lights == null) return;
            base.EvaluateQue(currentTime);

            lightColor = new Color(0,0,0,1);
            lightIntensity = 0f;
            spotAngle = 0f;
            innerSpotAngle = 0f;
            spotRange = 0f;
            lightCookie = null;
            while (stageLightDataQueue.Count>0)
            {
                var data = stageLightDataQueue.Dequeue();
                var stageLightBaseProperty= data.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var lightProperty = data.TryGetActiveProperty<LightProperty>() as LightProperty;
                var lightColorProperty = data.TryGetActiveProperty<LightColorProperty>() as LightColorProperty;
                var lightIntensityProperty = data.TryGetActiveProperty<LightIntensityProperty>() as LightIntensityProperty;
                var weight = data.weight;
                var stageLightOrderProperty = data.TryGetActiveProperty<StageLightOrderProperty>() as StageLightOrderProperty;
                var index =stageLightOrderProperty!=null? stageLightOrderProperty.stageLightOrderQueue.GetStageLightIndex(parentStageLight) :  parentStageLight.order;
                if(lightProperty == null || stageLightBaseProperty == null) continue;
             
                // Debug.Log($"{lightProperty.clockOverride.value.childStagger}, {lightProperty.clockOverride.value.propertyOverride}");
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightProperty),index);
                var manualLightArrayProperty = data.TryGetActiveProperty<ManualLightArrayProperty>();
                var manualColorArrayProperty = data.TryGetActiveProperty<ManualColorArrayProperty>();
                
                if (manualLightArrayProperty != null)
                {
                    var values = manualLightArrayProperty.lightValues.value;
                    if (index < values.Count)
                    {
                        var lightValue = values[index];
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
                        var t =lightIntensityProperty.clockOverride.propertyOverride ? SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightIntensityProperty),index) : normalizedTime;
                        lightIntensity += lightIntensityProperty.lightToggleIntensity.value.Evaluate(t) * weight;
                    }
                    spotAngle += lightProperty.spotAngle.value.Evaluate(normalizedTime) * weight;
                    innerSpotAngle += lightProperty.innerSpotAngle.value.Evaluate(normalizedTime) * weight;
                    spotRange += lightProperty.range.value.Evaluate(normalizedTime) * weight;
                }

                if (manualColorArrayProperty != null)
                {
                    var values = manualColorArrayProperty.colorValues.value;
                    if (index < values.Count)
                    {
                        var colorValue = values[index];
                        lightColor += colorValue.color * weight;
                    }
                    
                }else if (lightColorProperty != null)
                {
                    var t =lightColorProperty.clockOverride.propertyOverride ? SlmUtility.GetNormalizedTime(currentTime, data, typeof(LightColorProperty),index) : normalizedTime;
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
