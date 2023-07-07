//
// using StageLightManeuver;
// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(StageLightTimelineTrack))]
// [CanEditMultipleObjects]
// public class StageLightTimelineTrackEditor : UnityEditor.Editor
// {
//     
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         var target = serializedObject.targetObject;
//         var track = target as StageLightTimelineTrack;
//         EditorGUI.BeginChangeCheck();
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
//         if (EditorGUI.EndChangeCheck())
//         {
//             serializedObject.ApplyModifiedProperties();
//         }
//         // draw apply button
//         EditorGUILayout.BeginHorizontal();
//         EditorGUILayout.Space();
//         if (GUILayout.Button("Apply BPM", GUILayout.MaxWidth(100)))
//         {
//             track.ApplyBPM();
//         }
//         // EditorGUILayout.Space();
//         EditorGUILayout.EndHorizontal();
//
//         // space 
//         EditorGUILayout.Space();
//         var isChanged = SlmEditorUtility.DrawAddPropertyField( track.stageLightTimelineClips,"Add Property All Clip");
//
//         if (isChanged)
//         {
//             // set dirty
//             EditorUtility.SetDirty(target);
//             // AssetDatabase.SaveAssets();
//         }
//         // EditorGUILayout.Space();
//         // EditorGUILayout.BeginHorizontal();
//         // EditorGUILayout.Space();
//         // if (GUILayout.Button("Sort property", GUILayout.MaxWidth(100)))
//         // {
//         //     track.SortProperty();
//         // }
//         // // EditorGUILayout.Space();
//         // EditorGUILayout.EndHorizontal();
//     }
//         
//     
// }