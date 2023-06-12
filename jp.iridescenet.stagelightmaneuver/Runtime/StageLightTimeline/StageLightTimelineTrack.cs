using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System.Reflection;
#endif
namespace StageLightManeuver
{

    [TrackColor(0.8239978f, 0.9150943f, 0.3338079f)]
    [TrackClipType(typeof(StageLightTimelineClip))]
    [TrackBindingType(typeof(StageLightSupervisor))]
    public class StageLightTimelineTrack : TrackAsset
    {
        [Header("Base Settings")] [SerializeField]
        public float bpm = 120;

        [SerializeField] public float bpmScale = 1;
        [SerializeField] public string exportPath = "Assets/";

        [Header("Clip UI Options", order = 0)] [SerializeField] [Range(0, 1f)]
        public float colorLineHeight = 0.1f;

        [SerializeField] public bool drawBeat = true;
        [SerializeField] public Color beatLineColor = new Color(0, 1, 0.7126422f, 0.2f);
        [SerializeField] public bool updateOnOutOfClip = false;

        // private StageLightProfile referenceStageLightProfile;
        
        public List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
         // public List<SlmProperty> slmProperties;

#if UNITY_EDITOR
        
        private SerializedObject serializedProfile;
#endif

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            #if UNITY_EDITOR
            // serializedProfile = new SerializedObject(ReferenceStageLightProfile);
            // slmProperties = referenceStageLightProfile.stageLightProperties;
            #endif
            var mixer = ScriptPlayable<StageLightTimelineMixerBehaviour>.Create(graph, inputCount);
            var stageLightTimelineMixer = mixer.GetBehaviour();
            stageLightTimelineMixer.stageLightTimelineTrack = this;
            var clips = GetClips().ToList();
            stageLightTimelineMixer.clips = clips;
            var director = go.GetComponent<PlayableDirector>();
            foreach (var clip in clips)
            {
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                stageLightTimelineClip.track = this;
                stageLightTimelineClip.mixer = stageLightTimelineMixer;
                stageLightTimelineClip.clipDisplayName = clip.displayName;
            }

            return mixer;
        }

        public void OnEnable()
        {
            // if (referenceStageLightProfile == null)
            // {
            //     referenceStageLightProfile = ScriptableObject.CreateInstance<StageLightProfile>();
            //     referenceStageLightProfile.stageLightProperties = new List<SlmProperty>();
            // }
            //
            //
            // if (referenceStageLightProfile.stageLightProperties == null)
            //     referenceStageLightProfile.stageLightProperties = new List<SlmProperty>();
            //
            // if(serializedProfile == null)
            //     serializedProfile = new SerializedObject(ReferenceStageLightProfile);
#if UNITY_EDITOR
            Selection.selectionChanged -= OnSelection;
            Selection.selectionChanged += OnSelection;
#endif
            // slmProperties = referenceStageLightProfile.stageLightProperties;
        }

#if UNITY_EDITOR
        
        private void OnSelection()
        {
            
            var select = Selection.objects.ToList(); 
            selectedClips = new List<StageLightTimelineClip>();
            // selectedClips.Clear();
            foreach (var s in select)
            {
                if (s.GetType().ToString() == "UnityEditor.Timeline.EditorClip")
                {
                    var clip = s.GetType().GetField("m_Clip", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(s);
                    
                    var timelineClip = clip as TimelineClip;
                    if(timelineClip == null) continue;
                    if (timelineClip.asset.GetType() == typeof(StageLightTimelineClip))
                    {
                        // stringBuilder.AppendLine(timelineClip.displayName);
                        var asset = timelineClip.asset as StageLightTimelineClip;
                        
                        selectedClips.Add(asset);

                    }
                }
                
            }
        }
        
#endif
    }
}
