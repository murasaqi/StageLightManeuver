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
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverrideData);
            goboTexture = new SlmToggleValue<Texture2D>(other.goboTexture);
            goboPropertyName = new SlmToggleValue<string>(other.goboPropertyName);
            goroRotationSpeed = new SlmToggleValue<MinMaxEasingValue>(other.goroRotationSpeed);
        }
    }
}