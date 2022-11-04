#if USE_VLB_ALTER
using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class GoboProperty:SlmAdditionalProperty
    {
        [DisplayName("Gobo Texture")]public SlmToggleValue<Texture2D> goboTexture;
        [DisplayName("Gobo Property Name")]public SlmToggleValue<string> goboPropertyName;
        [DisplayName("Rotation Speed")]public SlmToggleValue<MinMaxEasingValue> goroRotationSpeed;

        public GoboProperty()
        {
            propertyName = "Gobo";
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            goboTexture = new SlmToggleValue<Texture2D>(){value = null};
            goboPropertyName = new SlmToggleValue<string>(){value = "_GoboTexture"};
            goroRotationSpeed = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle); 
            propertyOverride = toggle;
            goboTexture.propertyOverride = toggle;
            goboPropertyName.propertyOverride = toggle;
            goroRotationSpeed.propertyOverride = toggle;
        }

        public GoboProperty(GoboProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = other.bpmOverrideData.propertyOverride,
                value = new BpmOverrideToggleValueBase(other.bpmOverrideData.value)
            };
            goboTexture = new SlmToggleValue<Texture2D>(){value =  other.goboTexture.value};
            goboPropertyName = new SlmToggleValue<string>(){value = other.goboPropertyName.value};
            goroRotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.goroRotationSpeed.propertyOverride,
                value = new MinMaxEasingValue( other.goroRotationSpeed.value)
            };
        }
    }
}
#endif