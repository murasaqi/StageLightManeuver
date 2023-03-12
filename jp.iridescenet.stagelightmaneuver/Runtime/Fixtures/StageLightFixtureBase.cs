using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace StageLightManeuver
{
    
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightFixtureBase: MonoBehaviour,IStageLight
    {
        public List<Type> PropertyTypes = new List<Type>();
        public Queue<StageLightProfile> stageLightDataQueue = new Queue<StageLightProfile>();
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
        
        public float GetNormalizedTime(float time ,StageLightProfile queData, Type propertyType)
        {
            var additionalProperty = queData.TryGet(propertyType) as SlmAdditionalProperty;
            var timeProperty = queData.TryGet<ClockProperty>();
            var weight = queData.weight;
            if (additionalProperty == null || timeProperty == null) return 0f;
            var bpm = timeProperty.bpm.value;
            var bpmOffset = additionalProperty.clockOverride.value.propertyOverride ? additionalProperty.clockOverride.value.childStagger : timeProperty.childStagger.value;
            var bpmScale = additionalProperty.clockOverride.value.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : timeProperty.bpmScale.value;
            var loopType = additionalProperty.clockOverride.value.propertyOverride ? additionalProperty.clockOverride.value.loopType : timeProperty.loopType.value;
            var offsetTime = additionalProperty.clockOverride.value.propertyOverride
                ? additionalProperty.clockOverride.value.offsetTime
                : timeProperty.offsetTime.value;
            var clipProperty = timeProperty.clipProperty;
            var t = GetNormalizedTime(time+offsetTime,bpm,bpmOffset,bpmScale,clipProperty,loopType);
            return t;
        }


        public float GetOffsetTime(StageLightProfile queData, Type propertyType)
        {
            var additionalProperty = queData.TryGet(propertyType) as SlmAdditionalProperty;
            var timeProperty = queData.TryGet<ClockProperty>();
            var bpm = timeProperty.bpm.value;
            var bpmOffset = additionalProperty.clockOverride.value.propertyOverride ? additionalProperty.clockOverride.value.childStagger : timeProperty.childStagger.value;
            var bpmScale = additionalProperty.clockOverride.value.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : timeProperty.bpmScale.value;
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
