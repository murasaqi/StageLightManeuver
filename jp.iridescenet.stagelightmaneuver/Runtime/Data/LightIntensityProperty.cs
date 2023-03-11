using System;
using UnityEngine;

namespace StageLightManeuver
{

    [Serializable]
    public class LightIntensityProperty:SlmAdditionalProperty
    {
        [DisplayName("Intensity")]public SlmToggleValue<MinMaxEasingValue> lightToggleIntensity;// = new StageLightProperty<float>(){value = 1f};
        public LightIntensityProperty()
        {
            propertyName = "Intensity";
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = false,
                value = new BpmOverrideToggleValueBase()
            };
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                minMaxValue = new Vector2(0, 10),
                constant = 1f,
                mode = AnimationMode.Constant
            }};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            lightToggleIntensity.propertyOverride = toggle;
        }
        
        public LightIntensityProperty( LightIntensityProperty other )
        {
            propertyName = other.propertyName;
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverride);
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.lightToggleIntensity.propertyOverride,
                value = new MinMaxEasingValue(other.lightToggleIntensity.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is LightIntensityProperty)
            {
                var otherProperty = other as LightIntensityProperty;
                if (other.propertyOverride)
                {
                    if(otherProperty.lightToggleIntensity.propertyOverride) lightToggleIntensity.value = new MinMaxEasingValue(otherProperty.lightToggleIntensity.value);
                    if(otherProperty.bpmOverride.propertyOverride) bpmOverride.value = new BpmOverrideToggleValueBase(otherProperty.bpmOverride.value);
                }
                    
            }
        }
    }
}