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
            clockOverride = new SlmToggleValue<ClockOverrideToggleValueBase>()
            {
                propertyOverride = false,
                value = new ClockOverrideToggleValueBase()
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
            propertyOverride = toggle;
            clockOverride.propertyOverride = toggle;
            lightToggleIntensity.propertyOverride = toggle;
        }
        
        public LightIntensityProperty( LightIntensityProperty other )
        {
            propertyName = other.propertyName;
            clockOverride = new SlmToggleValue<ClockOverrideToggleValueBase>(other.clockOverride);
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
                    if(otherProperty.clockOverride.propertyOverride) clockOverride.value = new ClockOverrideToggleValueBase(otherProperty.clockOverride.value);
                }
                    
            }
        }
    }
}