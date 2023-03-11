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
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = false,
                value = new BpmOverrideToggleValueBase()
            };
            lightToggleColor = new SlmToggleValue<Gradient>(){value = new Gradient()};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            lightToggleColor.propertyOverride = toggle;
        }
        
        public LightColorProperty( LightColorProperty other )
        {
            propertyName = other.propertyName;
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverride);
            lightToggleColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = other.lightToggleColor.propertyOverride,
                value = SlmUtility.CopyGradient(other.lightToggleColor.value)
            };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            var otherProperty = other as LightColorProperty;
            if (otherProperty == null) return;
            if (other.propertyOverride)
            {
                if(otherProperty.lightToggleColor.propertyOverride) lightToggleColor.value = SlmUtility.CopyGradient(otherProperty.lightToggleColor.value);
                if(otherProperty.bpmOverride.propertyOverride) bpmOverride.value = new BpmOverrideToggleValueBase(otherProperty.bpmOverride.value);
            }
        }
    }
}