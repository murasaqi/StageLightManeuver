using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    
  
    [Serializable]
    [AddComponentMenu("")]
    public abstract class StageLightFixtureFixtureBase: MonoBehaviour,IStageLightFixture
    {
        public List<Type> PropertyTypes = new List<Type>();
        public Queue<StageLightQueueData> stageLightDataQueue = new Queue<StageLightQueueData>();
        public int updateOrder = 0;
        public List<StageLightFixtureBase> SyncStageLight { get; set; }
        public StageLight ParentStageLight { get; set; }
        public float offsetDuration = 0f;
        [FormerlySerializedAs("parentStageLightFixture")] [FormerlySerializedAs("parentStageLightFx")]
        public StageLight parentStageLight;
        // public int Index { get; set; }
        public virtual void EvaluateQue(float currentTime)
        {

        }

        public virtual void UpdateFixture()
        {
            
        }

        public virtual void Init()
        {
            SyncStageLight = new List<StageLightFixtureBase>();
            foreach (var stageLightFixtureBase in GetComponentsInChildren<StageLightFixtureBase>())
            {
                SyncStageLight.Add(stageLightFixtureBase);
            }
        }
        
    }
}
