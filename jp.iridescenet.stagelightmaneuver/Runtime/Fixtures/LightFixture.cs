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
                var weight = data.weight;
                if(lightProperty == null || stageLightBaseProperty == null) continue;
                
                var bpm = stageLightBaseProperty.bpm.value;
                var bpmOffset = lightProperty.bpmOverrideData.value.bpmOverride
                    ? lightProperty.bpmOverrideData.value.bpmOffset
                    : stageLightBaseProperty.bpmOffset.value;
                var bpmScale = lightProperty.bpmOverrideData.value.bpmOverride
                    ? lightProperty.bpmOverrideData.value.bpmScale
                    : stageLightBaseProperty.bpmScale.value;
                var loopType = lightProperty.bpmOverrideData.value.bpmOverride
                    ? lightProperty.bpmOverrideData.value.loopType
                    : stageLightBaseProperty.loopType.value;
                var clipProperty = stageLightBaseProperty.clipProperty;
                var t = GetNormalizedTime(currentTime,bpm,bpmOffset,bpmScale, clipProperty,loopType);
                lightColor += lightProperty.lightToggleColor.value.Evaluate(t) * weight;
                if (lightProperty.lightToggleIntensity.value.mode == AnimationMode.AnimationCurve)
                {
                    lightIntensity += lightProperty.lightToggleIntensity.value.animationCurve.Evaluate(t) * weight;
                }
                else if (lightProperty.lightToggleIntensity.value.mode == AnimationMode.Ease)
                {
                    lightIntensity += EaseUtil.GetEaseValue(lightProperty.lightToggleIntensity.value.easeType, t, 1f, lightProperty.lightToggleIntensity.value.rollRange.x,
                        lightProperty.lightToggleIntensity.value.rollRange.y) * weight;
                }else if (lightProperty.lightToggleIntensity.value.mode == AnimationMode.Constant)
                {
                    lightIntensity += lightProperty.lightToggleIntensity.value.constant * weight;
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
