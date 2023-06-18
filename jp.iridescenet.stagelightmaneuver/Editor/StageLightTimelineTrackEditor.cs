
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
        
        // draw apply button
        if (GUILayout.Button("Apply BPM"))
        {
            foreach (var target in targets)
            {
                var track = target as StageLightTimelineTrack;
                track.ApplyBPM();
            }
        }
    }
        
    
}