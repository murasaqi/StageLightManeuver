using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightFixtureBase: MonoBehaviour,IStageLight
    {
        public Type PropertyType = null;
        public Queue<StageLightQueData> stageLightDataQueue = new Queue<StageLightQueData>();
        public int updateOrder = 0;
        public int Index { get; set; }
        public List<StageLightBase> SyncStageLight { get; set; }
        public float offsetDuration = 0f;

        public virtual void EvaluateQue(float currentTime)
        {

        }

        public virtual void UpdateFixture()
        {
            
        }

        public virtual void Init()
        {
            
        }
        
        public float GetNormalizedTime(float time ,StageLightQueData queData, Type propertyType)
        {
            var additionalProperty = queData.TryGetAdditionalProperty(propertyType) as SlmAdditionalProperty;
            var timeProperty = queData.TryGet<TimeProperty>();
            var weight = queData.weight;
            if (additionalProperty == null || timeProperty == null) return 0f;
            var bpm = timeProperty.bpm.value;
            var bpmOffset = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.childStagger : timeProperty.childStagger.value;
            var bpmScale = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.bpmScale : timeProperty.bpmScale.value;
            var loopType = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.loopType : timeProperty.loopType.value;
            var offsetTime = additionalProperty.bpmOverrideData.value.bpmOverride
                ? additionalProperty.bpmOverrideData.value.offsetTime
                : timeProperty.offsetTime.value;
            var clipProperty = timeProperty.clipProperty;
            var t = GetNormalizedTime(time+offsetTime,bpm,bpmOffset,bpmScale,clipProperty,loopType);
            return t;
        }


        public float GetOffsetTime(StageLightQueData queData, Type propertyType)
        {
            var additionalProperty = queData.TryGetAdditionalProperty(propertyType) as SlmAdditionalProperty;
            var timeProperty = queData.TryGet<TimeProperty>();
            var bpm = timeProperty.bpm.value;
            var bpmOffset = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.childStagger : timeProperty.childStagger.value;
            var bpmScale = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.bpmScale : timeProperty.bpmScale.value;
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * (Index+1);
            return offset;
        }
        
        public float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType)
        {
            
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * (Index+1);
            var offsetTime = time + offset;
            offsetDuration = offset;
            var result = 0f;
            var t = (float)offsetTime % duration;
            var normalisedTime = t / duration;
            
            if (loopType == LoopType.Loop)
            {
                result = normalisedTime;     
            }else if (loopType == LoopType.PingPong)
            {
                result = Mathf.PingPong(offsetTime / duration, 1f);
            }
            else if(loopType == LoopType.Fixed)
            {
                result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, time);
            }
           
            return result;
        }
    }
}
