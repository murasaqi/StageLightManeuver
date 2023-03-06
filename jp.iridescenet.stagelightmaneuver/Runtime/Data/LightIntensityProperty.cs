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
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
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
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverrideData);
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.lightToggleIntensity.propertyOverride,
                value = new MinMaxEasingValue(other.lightToggleIntensity.value)
            };
        }
        
        
    }
}