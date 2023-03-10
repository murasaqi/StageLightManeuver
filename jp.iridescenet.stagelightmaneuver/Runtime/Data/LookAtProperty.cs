using System;

namespace StageLightManeuver
{
    [Serializable]
    public class LookAtProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<int> lookAtIndex;
        public SlmToggleValue<float> weight;
        public SlmToggleValue<float> speed;
        public LookAtProperty()
        {
            propertyName = "Look At";
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = false,
                value = new BpmOverrideToggleValueBase()
            };
            weight = new SlmToggleValue<float>(){value = 1f};
            lookAtIndex = new SlmToggleValue<int>(){value = 0};
            speed = new SlmToggleValue<float>(){value = 1f};
        }
        
        public override void ToggleOverride(bool toggle)
        {
            weight.propertyOverride = toggle;
        }
        
        public LookAtProperty( LookAtProperty other )
        {
            propertyName = other.propertyName;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverrideData);
            weight = new SlmToggleValue<float>(other.weight);
            lookAtIndex = new SlmToggleValue<int>(other.lookAtIndex);
            speed = new SlmToggleValue<float>(other.speed);
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            base.OverwriteProperty(other);
            LookAtProperty lookAtProperty = other as LookAtProperty;
            if (lookAtProperty == null) return;
            if(lookAtProperty.weight.propertyOverride) weight.value = lookAtProperty.weight.value;
            if(lookAtProperty.lookAtIndex.propertyOverride) lookAtIndex.value = lookAtProperty.lookAtIndex.value;
            if(lookAtProperty.speed.propertyOverride) speed.value = lookAtProperty.speed.value;
            if(lookAtProperty.bpmOverrideData.propertyOverride) bpmOverrideData.value = new BpmOverrideToggleValueBase(lookAtProperty.bpmOverrideData.value);
            
        }
    }
}