using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightColorProperty:SlmAdditionalProperty
    {
        [DisplayName("Color")]public SlmToggleValue<Gradient> lightToggleColor;// = new StageLightProperty<float>(){value = 1f};
        public LightColorProperty()
        {
            propertyName = "Light Color";
            lightToggleColor = new SlmToggleValue<Gradient>(){value = new Gradient()};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            lightToggleColor.propertyOverride = toggle;
        }
        
        public LightColorProperty( LightColorProperty other )
        {
            propertyName = other.propertyName;
            lightToggleColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = other.lightToggleColor.propertyOverride,
                value = SlmUtility.CopyGradient(other.lightToggleColor.value)
            };
        }
        
        
    }
}