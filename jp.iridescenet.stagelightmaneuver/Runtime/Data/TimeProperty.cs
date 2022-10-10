using System;

namespace StageLightManeuver
{
    [Serializable]
    public class TimeProperty: SlmProperty
    {
        [DisplayName("Clip Duration")] public ClipProperty clipProperty;
        [DisplayName("Loop Type")] public SlmToggleValue<LoopType> loopType;
        [DisplayName("BPM")]public SlmToggleValue<float> bpm;
        [DisplayName("BPM Scale")]public SlmToggleValue<float> bpmScale;
        [DisplayName("BPM Offset")]public SlmToggleValue<float> bpmOffset;
        
        public TimeProperty()
        {
            propertyName = "Time";
            propertyOverride = false;
            loopType = new SlmToggleValue<LoopType>(){value = LoopType.Loop};
            clipProperty = new ClipProperty(){clipStartTime = 0, clipEndTime = 0};
            bpm = new SlmToggleValue<float>() { value = 60 };
            bpmScale = new SlmToggleValue<float>() { value = 1f };
            bpmOffset = new SlmToggleValue<float>() { value = 0f };
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            loopType.propertyOverride = toggle;
            bpm.propertyOverride = toggle;
            bpmScale.propertyOverride = toggle;
            bpmOffset.propertyOverride = toggle;
            
        }

        public TimeProperty(TimeProperty other)
        {
            propertyOverride = other.propertyOverride;
            propertyName = other.propertyName;
            bpm = new SlmToggleValue<float>(other.bpm);
            bpmScale = new SlmToggleValue<float>(other.bpmScale);
            bpmOffset = new SlmToggleValue<float>(other.bpmOffset);
            loopType = new SlmToggleValue<LoopType>(other.loopType);
            clipProperty = new ClipProperty(other.clipProperty);
            
        }
    }
}