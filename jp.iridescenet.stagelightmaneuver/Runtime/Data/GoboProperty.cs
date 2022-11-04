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
            goboTexture = new SlmToggleValue<Texture2D>(){value =  other.goboTexture.value};
            goboPropertyName = new SlmToggleValue<string>(){value = other.goboPropertyName.value};
            goroRotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                    value = new MinMaxEasingValue()
                    {
                        valueRange = other.goroRotationSpeed.value.valueRange,
                        valueMinMax = other.goroRotationSpeed.value.valueMinMax,
                        easeType = other.goroRotationSpeed.value.easeType,
                        mode = other.goroRotationSpeed.value.mode,
                        animationCurve =  new AnimationCurve(other.goroRotationSpeed.value.animationCurve.keys)
                    }
            };
        }
    }
}