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
            colorPropertyName = new SlmToggleValue<string>(){value = "_MainColor"};
            materialindex = new SlmToggleValue<int>(){value = 0};
            color = new SlmToggleValue<Gradient>(){value =new Gradient()};
            intensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()};
        }
        
        public MaterialColorProperty(MaterialColorProperty materialColorProperty)
        {
            colorPropertyName = new SlmToggleValue<string>(){value = materialColorProperty.colorPropertyName.value};
            materialindex = new SlmToggleValue<int>(){value = materialColorProperty.materialindex.value};
            color = new SlmToggleValue<Gradient>(materialColorProperty.color);
            intensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue()
                {
                        animationCurve = new AnimationCurve(materialColorProperty.intensity.value.animationCurve.keys),
                        mode =  materialColorProperty.intensity.value.mode,
                        constant =  materialColorProperty.intensity.value.constant,
                        easeType =  materialColorProperty.intensity.value.easeType,
                        rollMinMax = new Vector2(materialColorProperty.intensity.value.rollMinMax.x,materialColorProperty.intensity.value.rollMinMax.y),
                        rollRange =     new Vector2(materialColorProperty.intensity.value.rollRange.x,materialColorProperty.intensity.value.rollRange.y),
                }
            };
        }
    }
}