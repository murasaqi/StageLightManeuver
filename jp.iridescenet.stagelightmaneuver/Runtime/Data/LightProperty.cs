using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class LightProperty: SlmAdditionalProperty
    {
        public SlmToggleValue<Gradient> lightToggleColor;// = new StageLightProperty<Gradient>(){value = new Gradient()};
        public SlmToggleValue<AnimationCurve> lightToggleIntensity;// = new StageLightProperty<float>(){value = 1f};
        public SlmToggleValue<float> spotAngle;// = new StageLightProperty<float>(){value = 15f};
        public SlmToggleValue<float> innerSpotAngle;// = new StageLightProperty<float>(){value = 10f};
        
        public LightProperty()
        {
            propertyName = "Light";
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            lightToggleColor = new SlmToggleValue<Gradient>(){value = new Gradient()};
            lightToggleIntensity = new SlmToggleValue<AnimationCurve>(){value = new AnimationCurve(new []{new Keyframe(0,0),new Keyframe(1,1)})};
            spotAngle = new SlmToggleValue<float>(){value = 15f};
            innerSpotAngle = new SlmToggleValue<float>(){value = 10f};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            lightToggleColor.propertyOverride = toggle;
            lightToggleIntensity.propertyOverride = toggle;
            spotAngle.propertyOverride = toggle;
            innerSpotAngle.propertyOverride = toggle;
            
        }

        public LightProperty(LightProperty other)
        {
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverrideData);
            lightToggleColor = new SlmToggleValue<Gradient>(other.lightToggleColor);
            lightToggleIntensity = new SlmToggleValue<AnimationCurve>(other.lightToggleIntensity);
            spotAngle = new SlmToggleValue<float>(other.spotAngle);
            innerSpotAngle = new SlmToggleValue<float>(other.innerSpotAngle);
        }
    }
}