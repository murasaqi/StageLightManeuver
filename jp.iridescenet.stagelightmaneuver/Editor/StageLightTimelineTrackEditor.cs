
using StageLightManeuver;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageLightTimelineTrack))]
[CanEditMultipleObjects]
public class StageLightTimelineTrackEditor : UnityEditor.Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var target = serializedObject.targetObject;
        var track = target as StageLightTimelineTrack;
        // draw apply button
        if (GUILayout.Button("Apply BPM"))
        {
            track.ApplyBPM();
        }

        SlmEditorUtility.DrawAddPropertyField( track.stageLightTimelineClips);
    }
        
    
}