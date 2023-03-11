
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [Serializable]
    public class RotationProperty : SlmAdditionalProperty
    {
        // public SlmToggleValue<Vector3> rotationAxis;
        [DisplayName("Rotation Speed")] public SlmToggleValue<MinMaxEasingValue> rotationSpeed;
        public RotationProperty()
        {
            propertyName = "Rotation";

            propertyOverride = false;
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
                { value = new BpmOverrideToggleValueBase() };
            // rotationAxis = new SlmToggleValue<Vector3>(){value = new Vector3(0,0,1)};
            rotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue(AnimationMode.Constant, new Vector2(-30,30), new Vector2(-40,40), EaseType.Linear,30, new AnimationCurve(new []{new Keyframe(0,0),new Keyframe(1,40)}))
            };
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            // rotationAxis.propertyOverride=(toggle);
            rotationSpeed.propertyOverride=(toggle);
            bpmOverride.propertyOverride=(toggle);
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            // if other propertyOverride is true, Override value if propertyOverride of property in other is true.
            if (other.propertyOverride) return;
            var rotationProperty = other as RotationProperty;
            if (rotationProperty == null) return;
            if (rotationProperty.propertyOverride) return;
            if(rotationProperty.rotationSpeed.propertyOverride) rotationSpeed.value = rotationProperty.rotationSpeed.value; 
            if(rotationProperty.bpmOverride.propertyOverride) bpmOverride.value = new BpmOverrideToggleValueBase(rotationProperty.bpmOverride.value);
            
        }

        public RotationProperty(RotationProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            bpmOverride = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride =  other.bpmOverride.propertyOverride,
                value = new BpmOverrideToggleValueBase(other.bpmOverride.value)
            };
            // rotationAxis = new SlmToggleValue<Vector3>(other.rotationAxis){};
            rotationSpeed = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.rotationSpeed.propertyOverride,
                value = new MinMaxEasingValue(other.rotationSpeed.value)
            };
        }

    }
}