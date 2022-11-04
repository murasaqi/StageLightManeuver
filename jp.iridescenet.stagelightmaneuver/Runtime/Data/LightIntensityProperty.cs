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
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                valueMinMax = new Vector2(0, 10),
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
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.lightToggleIntensity.propertyOverride,
                value = new MinMaxEasingValue(other.lightToggleIntensity.value)
            };
        }
        
        
    }
}