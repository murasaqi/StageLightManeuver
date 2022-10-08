using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using StageLightManeuver;

[Serializable]
public class StageLightTimelineBehaviour : PlayableBehaviour
{
    [SerializeField]
    public StageLightQueData stageLightQueData = new StageLightQueData();
    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
