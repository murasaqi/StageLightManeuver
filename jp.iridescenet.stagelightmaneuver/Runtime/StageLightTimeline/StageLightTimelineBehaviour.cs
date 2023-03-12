using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightTimelineBehaviour : PlayableBehaviour
    {
        [FormerlySerializedAs("stageLightQueData")] [SerializeField] public StageLightQueueData stageLightQueueData = new StageLightQueueData();
        // [SerializeField] public StageLightProfile stageLightProfile;
        public override void OnPlayableCreate(Playable playable)
        {
            // stageLightProfile = ScriptableObject.CreateInstance<StageLightProfile>();
            // stageLightProfile.name = "StageLightProfile";
            // stageLightProfile.stageLightProperties = new List<SlmProperty>();
        }
        
        
        public void Init()
        {
            stageLightQueueData = new StageLightQueueData();
            // stageLightQueueData.stageLightProperties = new List<SlmProperty>();
            // stageLightQueueData.stageLightProperties.Add(new ClockProperty());
        }
        public void RemoveNullProperties()
        {
            stageLightQueueData.stageLightProperties.RemoveAll(item => item == null);
        }
    }

}