using UnityEngine;

namespace StageLightManeuver
{
    public class MaterialColorProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<string> colorPropertyName;
        public SlmToggleValue<int> materialindex;
        public SlmToggleValue<Gradient> color;
        public SlmToggleValue<MinMaxEasingValue> intensity;

        public MaterialColorProperty()
        {
            propertyName = "Material Color";
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                value = new BpmOverrideToggleValueBase()
            };
            colorPropertyName = new SlmToggleValue<string>(){value = "_ShaderPropertyName"};
            materialindex = new SlmToggleValue<int>(){value = 0};
            color = new SlmToggleValue<Gradient>(){value =new Gradient()};
            intensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()};
        }
        
        public MaterialColorProperty(MaterialColorProperty materialColorProperty)
        {
            propertyName = materialColorProperty.propertyName;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                value = new BpmOverrideToggleValueBase()
                {
                    bpmOverride = materialColorProperty.bpmOverrideData.value.bpmOverride,
                    bpmScale = materialColorProperty.bpmOverrideData.value.bpmScale,
                    childStagger = materialColorProperty.bpmOverrideData.value.childStagger,
                    loopType = materialColorProperty.bpmOverrideData.value.loopType,
                    offsetTime = materialColorProperty.bpmOverrideData.value.offsetTime,
                    propertyOverride = materialColorProperty.bpmOverrideData.value.propertyOverride,
                }
            };
            colorPropertyName = new SlmToggleValue<string>(){value = materialColorProperty.colorPropertyName.value};
            materialindex = new SlmToggleValue<int>(){value = materialColorProperty.materialindex.value};
            
            var alphaKeys = new GradientAlphaKey[materialColorProperty.color.value.alphaKeys.Length];
            var colorKeys = new GradientColorKey[materialColorProperty.color.value.colorKeys.Length];
            color = new SlmToggleValue<Gradient>()
            {
                
                value = new Gradient()
                {
                    alphaKeys = alphaKeys,
                    colorKeys = colorKeys,
                    mode = materialColorProperty.color.value.mode
                }
            };
            intensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue()
                {
                        animationCurve = new AnimationCurve(materialColorProperty.intensity.value.animationCurve.keys),
                        mode =  materialColorProperty.intensity.value.mode,
                        constant =  materialColorProperty.intensity.value.constant,
                        easeType =  materialColorProperty.intensity.value.easeType,
                        valueMinMax = new Vector2(materialColorProperty.intensity.value.valueMinMax.x,materialColorProperty.intensity.value.valueMinMax.y),
                        valueRange =     new Vector2(materialColorProperty.intensity.value.valueRange.x,materialColorProperty.intensity.value.valueRange.y),
                }
            };
        }
    }
}