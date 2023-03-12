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
        public StageLightProfile stageLightProfile;
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

            InitStageLightProfile();
            var playable = ScriptPlayable<StageLightTimelineBehaviour>.Create(graph, behaviour);
            behaviour = playable.GetBehaviour();

            var queData = stageLightProfile;

            var timeProperty = queData.TryGet<ClockProperty>();
            if (timeProperty == null)
            {
                timeProperty = new ClockProperty();
                timeProperty.bpm.value = track.bpm;
                timeProperty.bpmScale.value = track.bpmScale;

                queData.stageLightProperties.Add(timeProperty);
                
                
                var playabledirector = owner.GetComponent<PlayableDirector>();
                // get parent track
            
                var propertyTypes = new List<Type>();
                foreach (var tAssetOutput in playabledirector.playableAsset.outputs)
                {
                    if(tAssetOutput.sourceObject.GetType() == typeof(StageLightTimelineTrack))
                    {
                        var track = tAssetOutput.sourceObject as TrackAsset;
                        foreach (var timelineClip in track.GetClips())
                        {
                            var stageLightTimelineClip = timelineClip.asset as StageLightTimelineClip;
                            if (stageLightTimelineClip == this)
                            {
                                var binding = playabledirector.GetGenericBinding(track);
                                if (binding != null)
                                {
                                    var stageLightSupervisor = binding as StageLightSupervisor;
                                    if (stageLightSupervisor != null)
                                    {
                                        propertyTypes.AddRange(stageLightSupervisor.GetAllPropertyType());
                                    }
                                }
                            }
                        }
                    }
                }
                propertyTypes = propertyTypes.Distinct().ToList();

                foreach (var propertyType in propertyTypes)
                {
                    // if not contain fixture type in queData, add it
                
                    if (queData.stageLightProperties.Find( x => x.GetType() == propertyType) == null)
                    {
                        var fixture = Activator.CreateInstance(propertyType) as SlmProperty;
                        queData.stageLightProperties.Add(fixture);
                    }
                }

            }
            
            // Debug.Log(owner);
            
            // Find StageLightSupervisor from owner
           

            if (syncReferenceProfile && referenceStageLightProfile != null)
            {
                InitSyncData();
            }

            return playable;
        }

        
        public void InitStageLightProfile()
        {
            stageLightProfile = ScriptableObject.CreateInstance<StageLightProfile>();
            stageLightProfile.name = "StageLightProfile";
            stageLightProfile.stageLightProperties = new List<SlmProperty>();
            stageLightProfile.stageLightProperties.Add(new ClockProperty());
            // stageLightProfile.stageLightProperties.AddRange(behaviour.stageLightQueData.stageLightProperties);
        }
        // private void SetInitValues()
        // {
        //     foreach (var VARIABLE in behaviour.stageLightQueData.stageLightProperties)
        //     {
        //         
        //     }
        // }


        [ContextMenu("Apply")]
        public void LoadProfile()
        {
            if (referenceStageLightProfile == null || syncReferenceProfile) return;

            var hasTimeProperty = false;
           
            
            var copy = new List<SlmProperty>();
            foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
            {
                if(stageLightProperty == null) continue;
                var type = stageLightProperty.GetType();
                copy.Add(Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                        new object[] { stageLightProperty }, null)
                    as SlmProperty);
            }

            var timeProperty = copy.Find(x => x.GetType() == typeof(ClockProperty));

            if (timeProperty == null)
            {
                copy.Insert(0, new ClockProperty());
            }
            
            stageLightProfile.stageLightProperties = copy;
                


        }

        public void SaveProfile()
        {
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(referenceStageLightProfile, referenceStageLightProfile.name);
            var copy = new List<SlmProperty>();
            foreach (var stageLightProperty in stageLightProfile.stageLightProperties)
            {
                if(stageLightProperty ==null) continue;
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
                        if(stageLightProperty == null) continue;
                        stageLightProperty.propertyOverride = true;
                    }

                    stageLightProfile.stageLightProperties =
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
                        if(stageLightProperty == null) continue;
                        var type = stageLightProperty.GetType();
                        copy.Add(Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                                new object[] { stageLightProperty }, null)
                            as SlmProperty);
                    }

                    stageLightProfile.stageLightProperties = copy;
                }
            }
        }


    }
}
