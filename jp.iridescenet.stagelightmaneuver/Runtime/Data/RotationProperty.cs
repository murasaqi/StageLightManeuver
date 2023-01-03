
using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class RotationProperty : SlmAdditionalProperty
    {
        public SlmToggleValue<Vector3> rotationAxis;
        public SlmToggleValue<float> rotationScalar;
        public RotationProperty()
        {
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
                { value = new BpmOverrideToggleValueBase() };
            rotationAxis = new SlmToggleValue<Vector3>(){value = Vector3.zero};
            rotationScalar = new SlmToggleValue<float>(){value = 0};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
            rotationAxis.propertyOverride=(toggle);
            rotationScalar.propertyOverride=(toggle);
            bpmOverrideData.propertyOverride=(toggle);
        }
        
        public RotationProperty(RotationProperty other)
        {
            propertyOverride = other.propertyOverride;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride =  other.bpmOverrideData.propertyOverride,
                value = new BpmOverrideToggleValueBase(other.bpmOverrideData.value)
            };
            rotationAxis = new SlmToggleValue<Vector3>(other.rotationAxis){};
            rotationScalar = new SlmToggleValue<float>(other.rotationScalar){};
        }

    }
}