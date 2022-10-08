using System;

namespace StageLightManeuver
{
    [Serializable]
    public class TimeProperty: SlmProperty
    {
        [DisplayName("Clip Duration")] public ClipProperty clipProperty;
        [DisplayName("Loop Type")] public SlmToggleValue<LoopType> loopType = new SlmToggleValue<LoopType>() { value = LoopType.Loop };
        [DisplayName("BPM")]public SlmToggleValue<float> bpm = new SlmToggleValue<float>() { value = 120 };
        [DisplayName("BPM Scale")]public SlmToggleValue<float> bpmScale = new SlmToggleValue<float>(){value = 1f};
        [DisplayName("BPM Offset")]public SlmToggleValue<float> bpmOffset = new SlmToggleValue<float>() { value = 0f };
        
        public TimeProperty()
        {
            propertyName = "Time";
            propertyOverride = false;
            loopType = new SlmToggleValue<LoopType>(){value = LoopType.Loop};
            clipProperty = new ClipProperty(){clipStartTime = 0, clipEndTime = 0};
            bpm = new SlmToggleValue<float>() { value = 120 };
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