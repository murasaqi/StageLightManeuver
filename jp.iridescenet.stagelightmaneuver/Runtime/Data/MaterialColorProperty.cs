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
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                value = new BpmOverrideToggleValueBase()
            };
            colorPropertyName = new SlmToggleValue<string>(){value = "_ShaderPropertyName"};
            materialindex = new SlmToggleValue<int>(){value = 0};
            color = new SlmToggleValue<Gradient>(){value =new Gradient()};
            intensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxLimit = new Vector2(0,2),
                mode = AnimationMode.Constant,
                constant = 1
            }};
        }
        
        public MaterialColorProperty(MaterialColorProperty materialColorProperty)
        {
            propertyName = materialColorProperty.propertyName;
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride =  materialColorProperty.bpmOverride.propertyOverride,
                value = new BpmOverrideToggleValueBase(materialColorProperty.bpmOverride.value)
            };
            colorPropertyName = new SlmToggleValue<string>(){value = materialColorProperty.colorPropertyName.value};
            materialindex = new SlmToggleValue<int>(){value = materialColorProperty.materialindex.value};
            
           color = new SlmToggleValue<Gradient>()
            {
                propertyOverride =  materialColorProperty.color.propertyOverride,
                value = SlmUtility.CopyGradient( materialColorProperty.color.value)
            };
            intensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = materialColorProperty.intensity.propertyOverride,
                value = new MinMaxEasingValue(materialColorProperty.intensity.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            MaterialColorProperty materialColorProperty = other as MaterialColorProperty;
            if (materialColorProperty == null) return;
            if(materialColorProperty.colorPropertyName.propertyOverride) colorPropertyName.value = materialColorProperty.colorPropertyName.value;
            if(materialColorProperty.materialindex.propertyOverride) materialindex.value = materialColorProperty.materialindex.value;
            if(materialColorProperty.color.propertyOverride) color.value = SlmUtility.CopyGradient(materialColorProperty.color.value);
            if(materialColorProperty.intensity.propertyOverride) intensity.value = new MinMaxEasingValue(materialColorProperty.intensity.value);
            if(materialColorProperty.bpmOverride.propertyOverride) bpmOverride.value = new BpmOverrideToggleValueBase(materialColorProperty.bpmOverride.value);
        }
    }
}