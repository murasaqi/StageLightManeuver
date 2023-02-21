using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEditor;

namespace StageLightManeuver
{

    
    

    [Serializable]
    public class StageLightTimelineClip : PlayableAsset, ITimelineClipAsset
    {

        public StageLightProfile referenceStageLightProfile;
        [HideInInspector] public StageLightTimelineBehaviour behaviour = new StageLightTimelineBehaviour();
        public bool forceTimelineClipUpdate;
        public bool syncReferenceProfile = false;
        public StageLightTimelineTrack track;
        public string exportPath = "";
        public StageLightTimelineMixerBehaviour mixer;
        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public string clipDisplayName;


        public void OnEnable()
        {

        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {


            var playable = ScriptPlayable<StageLightTimelineBehaviour>.Create(graph, behaviour);
            behaviour = playable.GetBehaviour();

            var queData = behaviour.stageLightQueData;

            var timeProperty = queData.TryGet<TimeProperty>();
            if (timeProperty == null)
            {
                timeProperty = new TimeProperty();
                timeProperty.bpm.value = track.bpm;
                timeProperty.bpmScale.value = track.bpmScale;

                behaviour.stageLightQueData.stageLightProperties.Add(timeProperty);
            }
            

            if (syncReferenceProfile && referenceStageLightProfile != null)
            {
                InitSyncData();
            }

            return playable;
        }


        [ContextMenu("Apply")]
        public void LoadProfile()
        {
            if (referenceStageLightProfile == null || syncReferenceProfile) return;

            var hasTimeProperty = false;
            var props = referenceStageLightProfile.Clone().stageLightProperties;

            behaviour.stageLightQueData.stageLightProperties = props;

            foreach (var prop in props)
            {
                if (prop is TimeProperty)
                {
                    hasTimeProperty = true;
                }
            }

            if (hasTimeProperty == false)
            {
                behaviour.stageLightQueData.stageLightProperties.Insert(0, new TimeProperty());
            }


        }

        public void SaveProfile()
        {
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(referenceStageLightProfile, referenceStageLightProfile.name);
            var copy = new List<SlmProperty>();
            foreach (var stageLightProperty in behaviour.stageLightQueData.stageLightProperties)
            {
                var type = stageLightProperty.GetType();
                copy.Add(Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                        new object[] { stageLightProperty }, null)
                    as SlmProperty);
            }

            referenceStageLightProfile.stageLightProperties.Clear();
            referenceStageLightProfile.stageLightProperties = copy;
            referenceStageLightProfile.isUpdateGuiFlag = true;
            EditorUtility.SetDirty(referenceStageLightProfile);
            AssetDatabase.SaveAssets();
#endif
        }

        public void InitSyncData()
        {
            if (syncReferenceProfile)
            {
                if (referenceStageLightProfile != null)
                {

                    foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
                    {
                        stageLightProperty.ToggleOverride(true);
                        stageLightProperty.propertyOverride = true;
                    }

                    behaviour.stageLightQueData.stageLightProperties =
                        referenceStageLightProfile.stageLightProperties;
                }
            }
            else
            {
                if (referenceStageLightProfile != null)
                {

                    var copy = new List<SlmProperty>();
                    foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
                    {
                        var type = stageLightProperty.GetType();
                        copy.Add(Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                                new object[] { stageLightProperty }, null)
                            as SlmProperty);
                    }

                    behaviour.stageLightQueData.stageLightProperties = copy;
                }
            }
        }


    }
}
