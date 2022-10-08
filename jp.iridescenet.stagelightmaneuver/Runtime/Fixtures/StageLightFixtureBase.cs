using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
  
    [Serializable]
    public abstract class StageLightFixtureBase: MonoBehaviour,IStageLight
    {
        // public StageLightBaseProperty stageLightBaseProperty = new StageLightBaseProperty();
        // public ClipProperty clipProperty = new ClipProperty();
        public Queue<StageLightQueData> stageLightDataQueue = new Queue<StageLightQueData>();

        public int Index { get; set; }
        public List<StageLight> StageLightChild { get; set; }
        public float offsetDuration = 0f;

        public virtual void UpdateFixture(float currentTime)
        {

        }

        public virtual void Init()
        {
            
        }
        
        
        public float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType)
        {
            
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * Index;
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
