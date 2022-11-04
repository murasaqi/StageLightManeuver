using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using StageLightManeuver;
using UnityEditor;


[Serializable]
public class StageLightTimelineClip : PlayableAsset, ITimelineClipAsset
{
    
    public StageLightProfile referenceStageLightProfile;
    [HideInInspector]public StageLightTimelineBehaviour stageLightTimelineBehaviour = new StageLightTimelineBehaviour ();
    public bool forceTimelineClipUpdate;
    public bool syncReferenceProfile = true;
    public StageLightTimelineTrack track;
    public string exportPath = "Assets/";
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }
    
    
    public void OnEnable()
    {

    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        
        
        var playable = ScriptPlayable<StageLightTimelineBehaviour>.Create (graph, stageLightTimelineBehaviour);
        stageLightTimelineBehaviour = playable.GetBehaviour ();

        var queData = stageLightTimelineBehaviour.stageLightQueData;

        var timeProperty = queData.TryGet<TimeProperty>();
        if(timeProperty == null)
        {
            timeProperty = new TimeProperty();
            timeProperty.bpm.value = track.bpm;
            timeProperty.bpmScale.value = track.bpmScale;
            
            stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(timeProperty);
        }

        if (exportPath == "")
        {
            exportPath = $"{track.exportPath}{this.name}.asset";
        }
        
        if(syncReferenceProfile && referenceStageLightProfile != null)
        {
            InitSyncData();
        }
        return playable;
    }
    
    
    [ContextMenu("Apply")]
    public void LoadProfile()
    {
        if (referenceStageLightProfile == null || syncReferenceProfile) return;
        stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Clear();

        var hasTimeProperty = false;
        var props = new SlmProperty[referenceStageLightProfile.stageLightProperties.Count];
        referenceStageLightProfile.stageLightProperties.CopyTo(props);      
        foreach (var prop in props)
        {
            if (prop is TimeProperty)
            {
                hasTimeProperty = true;
            }
            stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(prop);
        }
        
        if(hasTimeProperty == false)
        {
            stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Insert(0,new TimeProperty());
        }
    }

    public void SaveProfile()
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(referenceStageLightProfile, referenceStageLightProfile.name);
        
        var props = new SlmProperty[stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Count];
        stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.CopyTo(props);
        referenceStageLightProfile.stageLightProperties = props.ToList();
        referenceStageLightProfile.isUpdateGuiFlag = true;
        EditorUtility.SetDirty(referenceStageLightProfile);
        AssetDatabase.SaveAssets();
#endif
    }

    public void InitSyncData()
    {
        if (syncReferenceProfile)
        {
            if(referenceStageLightProfile != null)
            {
                
                foreach (var stageLightProperty in referenceStageLightProfile.stageLightProperties)
                {
                    stageLightProperty.ToggleOverride(true);
                    stageLightProperty.propertyOverride = true;
                }
                stageLightTimelineBehaviour.stageLightQueData.stageLightProperties =
                    referenceStageLightProfile.stageLightProperties;    
            }
        }
        else
        {
            if(referenceStageLightProfile != null)
            {
                var props = new SlmProperty[referenceStageLightProfile.stageLightProperties.Count];
                referenceStageLightProfile.stageLightProperties.CopyTo(props);  
                stageLightTimelineBehaviour.stageLightQueData.stageLightProperties = props.ToList();
            }
        }
    }
}
