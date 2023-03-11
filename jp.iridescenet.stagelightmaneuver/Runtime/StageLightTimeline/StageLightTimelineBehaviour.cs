using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightTimelineBehaviour : PlayableBehaviour
    {
        [SerializeField] public StageLightQueData stageLightQueData = new StageLightQueData();
        // public StageLightProfile stageLightProfile;
        public override void OnPlayableCreate(Playable playable)
        {
            // stageLightProfile = ScriptableObject.CreateInstance<StageLightProfile>();
            // stageLightProfile.name = "StageLightProfile";
            // stageLightProfile.stageLightProperties = new List<SlmProperty>();
        }
    }

}