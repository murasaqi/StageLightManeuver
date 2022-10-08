using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using StageLightManeuver;

[TrackColor(0.8239978f, 0.9150943f, 0.3338079f)]
[TrackClipType(typeof(StageLightTimelineClip))]
[TrackBindingType(typeof(StageLight))]
public class StageLightTimelineTrack : TrackAsset
{
    [Header("Clip UI Options", order = 0)] 
    [SerializeField][Range(0,1f)]public float colorLineHeight = 0.1f;
    [SerializeField]public bool drawBeat =false;
    [SerializeField] public Color beatLineColor = new Color(0, 1, 0.7126422f, 0.3f);
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<StageLightTimelineMixerBehaviour>.Create (graph, inputCount);
        var stageLightTimelineMixer = mixer.GetBehaviour();
        var clips = GetClips().ToList();
        stageLightTimelineMixer.clips = clips;
        var director = go.GetComponent<PlayableDirector>();
        foreach (var clip in clips)
        {
            var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
            stageLightTimelineClip.track = this;
        }

        return mixer;
    }
}
