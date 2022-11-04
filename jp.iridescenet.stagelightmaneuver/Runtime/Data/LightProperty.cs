using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightProperty: SlmAdditionalProperty
    {
        [DisplayName("Spot Angle")]public SlmToggleValue<float> spotAngle;// = new StageLightProperty<float>(){value = 15f};
        [DisplayName("Inner Spot Angle")]public SlmToggleValue<float> innerSpotAngle;// = new StageLightProperty<float>(){value = 10f};
        [DisplayName("Range")]public SlmToggleValue<float> range;
        public LightProperty()
        {
            propertyName = "Light";
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            spotAngle = new SlmToggleValue<float>(){value = 15f};
            innerSpotAngle = new SlmToggleValue<float>(){value = 10f};
            range = new SlmToggleValue<float>(){value = 10f};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            spotAngle.propertyOverride = toggle;
            innerSpotAngle.propertyOverride = toggle;
            range.propertyOverride = toggle;
            
        }

        public LightProperty(LightProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride =  other.bpmOverrideData.propertyOverride,
                value =  new BpmOverrideToggleValueBase(other.bpmOverrideData.value),
            };
            spotAngle = new SlmToggleValue<float>(other.spotAngle);
            innerSpotAngle = new SlmToggleValue<float>(other.innerSpotAngle);
            range = new SlmToggleValue<float>(other.range);
        }
    }
}