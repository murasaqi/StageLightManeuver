using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class LightFixture : StageLightFixtureBase
    {
        public Light light;
        public Color lightColor;
        public float lightIntensity;
        public float spotAngle;
        public float innerSpotAngle;
        public float spotRange;
        public UniversalAdditionalLightData universalAdditionalLightData;
        
        public override void EvaluateQue(float currentTime)
        {
            if(light == null) return;
            base.EvaluateQue(currentTime);

            lightColor = new Color(0,0,0,1);
            lightIntensity = 0f;
            spotAngle = 0f;
            innerSpotAngle = 0f;
            spotRange = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var data = stageLightDataQueue.Dequeue();
                var stageLightBaseProperty= data.TryGet<TimeProperty>() as TimeProperty;
                var lightProperty = data.TryGet<LightProperty>() as LightProperty;
                var lightColorProperty = data.TryGet<LightColorProperty>() as LightColorProperty;
                var lightIntensityProperty = data.TryGet<LightIntensityProperty>() as LightIntensityProperty;
                var weight = data.weight;
                if(lightProperty == null || stageLightBaseProperty == null) continue;
             
                var baseTime = GetNormalizedTime(currentTime, data, typeof(LightProperty));
               
                if (lightColorProperty != null)
                {
                    var t =lightColorProperty.bpmOverrideData.value.bpmOverride ? GetNormalizedTime(currentTime, data, typeof(LightColorProperty)) : baseTime;
                    lightColor += lightColorProperty.lightToggleColor.value.Evaluate(t) * weight;
                    
                }

                if (lightIntensityProperty != null)
                {
                    var t =lightIntensityProperty.bpmOverrideData.value.bpmOverride ? GetNormalizedTime(currentTime, data, typeof(LightIntensityProperty)) : baseTime;
                    lightIntensity += lightIntensityProperty.lightToggleIntensity.value.Evaluate(t) * weight;
                }

                spotAngle += lightProperty.spotAngle.value * weight;
                innerSpotAngle += lightProperty.innerSpotAngle.value * weight;
                spotRange += lightProperty.range.value * weight;
            }
        }

        public override void UpdateFixture()
        {
            if (light==null) return; 
            light.color = lightColor;
            light.intensity = lightIntensity;
            light.spotAngle = spotAngle;
            light.innerSpotAngle = innerSpotAngle;
            light.range = spotRange;
        }
    }
}
