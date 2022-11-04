using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightProperty: SlmAdditionalProperty
    {
        [DisplayName("Color")]public SlmToggleValue<Gradient> lightToggleColor;// = new StageLightProperty<Gradient>(){value = new Gradient()};
        [DisplayName("Intensity")]public SlmToggleValue<MinMaxEasingValue> lightToggleIntensity;// = new StageLightProperty<float>(){value = 1f};
        [DisplayName("Spot Angle")]public SlmToggleValue<float> spotAngle;// = new StageLightProperty<float>(){value = 15f};
        [DisplayName("Inner Spot Angle")]public SlmToggleValue<float> innerSpotAngle;// = new StageLightProperty<float>(){value = 10f};
        [DisplayName("Range")]public SlmToggleValue<float> range;
        public LightProperty()
        {
            propertyName = "Light";
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            lightToggleColor = new SlmToggleValue<Gradient>(){value = new Gradient()};
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>(){value = new MinMaxEasingValue()
            {
                valueMinMax = new Vector2(0,10),
                valueRange = new Vector2(0,1)
            }};
            spotAngle = new SlmToggleValue<float>(){value = 15f};
            innerSpotAngle = new SlmToggleValue<float>(){value = 10f};
            range = new SlmToggleValue<float>(){value = 10f};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            lightToggleColor.propertyOverride = toggle;
            lightToggleIntensity.propertyOverride = toggle;
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
            lightToggleColor = new SlmToggleValue<Gradient>()
            {
                propertyOverride = other.lightToggleColor.propertyOverride,
                value = SlmUtility.CopyGradient(other.lightToggleColor.value)
            };
            var intensity = other.lightToggleIntensity.value;
            lightToggleIntensity = new SlmToggleValue<MinMaxEasingValue>()
            {
                value = new MinMaxEasingValue(intensity.mode,intensity.valueMinMax,intensity.valueMinMax,intensity.easeType,intensity.constant,intensity.animationCurve )
            };
            spotAngle = new SlmToggleValue<float>(other.spotAngle);
            innerSpotAngle = new SlmToggleValue<float>(other.innerSpotAngle);
            range = new SlmToggleValue<float>(other.range);
        }
    }
}