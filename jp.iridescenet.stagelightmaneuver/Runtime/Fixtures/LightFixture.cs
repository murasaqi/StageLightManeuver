using System;
using System.Collections.Generic;
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
    [AddComponentMenu("")]
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
#if USE_VLB
        public VolumetricLightBeamHD volumetricLightBeamHd;
        public VolumetricCookieHD volumetricCookieHd;
#endif
        // public UniversalAdditionalLightData universalAdditionalLightData;

        public override void Init()
        {
            base.Init();
#if USE_HDRP
            lightData.Clear();
#endif
            foreach (var light in lights)
            {
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
            }
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
                var stageLightBaseProperty= data.TryGet<TimeProperty>() as TimeProperty;
                var lightProperty = data.TryGet<LightProperty>() as LightProperty;
                var lightColorProperty = data.TryGet<LightColorProperty>() as LightColorProperty;
                var lightIntensityProperty = data.TryGet<LightIntensityProperty>() as LightIntensityProperty;
                var weight = data.weight;
                if(lightProperty == null || stageLightBaseProperty == null) continue;
             
                var normalizedTime = GetNormalizedTime(currentTime, data, typeof(LightProperty));
                var manualLightArrayProperty = data.TryGet<ManualLightArrayProperty>();

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
                        var t =lightIntensityProperty.bpmOverrideData.value.bpmOverride ? GetNormalizedTime(currentTime, data, typeof(LightIntensityProperty)) : normalizedTime;
                        lightIntensity += lightIntensityProperty.lightToggleIntensity.value.Evaluate(t) * weight;
                    }

                    spotAngle += lightProperty.spotAngle.value.Evaluate(normalizedTime) * weight;
                    innerSpotAngle += lightProperty.innerSpotAngle.value.Evaluate(normalizedTime) * weight;
                    spotRange += lightProperty.range.value.Evaluate(normalizedTime) * weight;
    
                }
                
                
                if (lightColorProperty != null)
                {
                    var t =lightColorProperty.bpmOverrideData.value.bpmOverride ? GetNormalizedTime(currentTime, data, typeof(LightColorProperty)) : normalizedTime;
                    lightColor += lightColorProperty.lightToggleColor.value.Evaluate(t) * weight;
                    
                }

                if (weight > 0.5f)
                {
                    lightCookie = lightProperty.cookie.value;
#if USE_VLB
                    volumetricCookieHd.cookieTexture = lightProperty.cookie.value;
#endif
                }
            }
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
    }
}
