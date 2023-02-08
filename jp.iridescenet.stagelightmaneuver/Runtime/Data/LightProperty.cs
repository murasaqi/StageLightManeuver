using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightProperty: SlmAdditionalProperty
    {
        [DisplayName("Spot Angle")]public SlmToggleValue<MinMaxEasingValue> spotAngle;// = new StageLightProperty<float>(){value = 15f};
        [DisplayName("Inner Spot Angle")]public SlmToggleValue<MinMaxEasingValue> innerSpotAngle;// = new StageLightProperty<float>(){value = 10f};
        [DisplayName("Range")]public SlmToggleValue<MinMaxEasingValue> range;
        [DisplayName("Cookie")]public SlmToggleValue<Texture> cookie;
        public LightProperty()
        {
            propertyName = "Light";
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            spotAngle = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                valueMinMax =  new Vector2(0,180),
                constant = 30f,
                mode = AnimationMode.Constant
            }};
            innerSpotAngle = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                valueMinMax =  new Vector2(0,180),
                constant = 10f,
                mode = AnimationMode.Constant
            }};
            range =  new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                valueMinMax =  new Vector2(0,100),
                constant = 5f,
                mode = AnimationMode.Constant
            }};
            cookie = new SlmToggleValue<Texture>(){value = null};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            spotAngle.propertyOverride = toggle;
            innerSpotAngle.propertyOverride = toggle;
            range.propertyOverride = toggle;
            cookie.propertyOverride = toggle;
            
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
            spotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.spotAngle.propertyOverride,
                value = new MinMaxEasingValue(other.spotAngle.value)
            };
            innerSpotAngle = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.innerSpotAngle.propertyOverride,
                value = new MinMaxEasingValue(other.innerSpotAngle.value)
            };
            range = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride = other.range.propertyOverride,
                value = new MinMaxEasingValue(other.range.value)
            };
            cookie = new SlmToggleValue<Texture>(other.cookie);
        }
    }
}