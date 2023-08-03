using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightManeuverSettings), true)]
    public class StageLightManeuverSettingsEditor : Editor
    {
        public StageLightManeuverSettings stageLightManeuverSettings;
        private ReorderableList reorderableSlmPropertys;
        private bool _isExpandedReorderablePropertys = false;
        private List<Type> slmPropertyTypes;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            stageLightManeuverSettings = target as StageLightManeuverSettings;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("exportProfilePath"));
            // Draw line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (reorderableSlmPropertys == null)
            {
                var slmPropertys = stageLightManeuverSettings.SlmPropertyOrder
                                        .OrderBy(x => x.Value)
                                        .ToDictionary(x => x.Key, x => x.Value).Keys
                                        .ToList();
                reorderableSlmPropertys = new ReorderableList(slmPropertys, typeof(Type), true, true, false, false);
                reorderableSlmPropertys.drawHeaderCallback = (rect) =>
                {
                    rect.x += 10;
                    _isExpandedReorderablePropertys = EditorGUI.Foldout(rect, _isExpandedReorderablePropertys, "SlmProperty Order");
                };
                reorderableSlmPropertys.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = reorderableSlmPropertys.list[index];
                    // 型名を表示
                    EditorGUI.LabelField(rect, element.ToString().Split('.').Last());
                };
                reorderableSlmPropertys.onSelectCallback = (list) =>
                {
                    if (list.index == 0 || list.index == 1) { reorderableSlmPropertys.ClearSelection(); }
                };
            }

            if (_isExpandedReorderablePropertys)
            {
                reorderableSlmPropertys.DoLayoutList();
            }
            else
            {
                // exec private method DoListHeader() from ReorderableList by reflection
                var rect = EditorGUILayout.GetControlRect();
                var method = reorderableSlmPropertys.GetType().GetMethod("DoListHeader", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(reorderableSlmPropertys, new object[] { rect });
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            // "apply", "revert", "reset to default" button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var buttonMinWidth = 60;
                if (GUILayout.Button("Save", GUILayout.MinWidth(buttonMinWidth)))
                {
                    UpdatePropertyOrder();
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Repaint();
                    ReloadPreviewInstances();
                    // Debug.Log("[StageLightManeuverSettings] Apply modified settings");
                }
                //TODO: revert と reset の実装

                // if (GUILayout.Button("Revert", GUILayout.MinWidth(buttonMinWidth)))
                // {
                //     serializedObject.Update();
                //     AssetDatabase.SaveAssets();
                //     AssetDatabase.Refresh();
                //     Repaint();
                //     ReloadPreviewInstances();
                //     Debug.Log("[StageLightManeuverSettings] Revert modified settings");
                // }
                // if (GUILayout.Button("Reset", GUILayout.MinWidth(buttonMinWidth)))
                // {
                //     stageLightManeuverSettings = StageLightManeuverSettings.CreateInstance<StageLightManeuverSettings>();;
                //     UpdatePropertyOrder();
                //     AssetDatabase.SaveAssets();
                //     AssetDatabase.Refresh();
                //     Debug.Log("[StageLightManeuverSettings] Reset to default");
                // }
                GUILayout.Space(4);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(stageLightManeuverSettings, "StageLightManeuverSettings");
            }
        }

        private void UpdatePropertyOrder()
        {
            var slmPropertyOrder = stageLightManeuverSettings.SlmPropertyOrder;
            for (int i = 0; i < reorderableSlmPropertys.list.Count; i++)
            {
                var type = reorderableSlmPropertys.list[i] as Type;
                slmPropertyOrder[type] = i;
            }
            slmPropertyOrder[typeof(ClockProperty)] = -999;
            slmPropertyOrder[typeof(StageLightOrderProperty)] = -998;
            stageLightManeuverSettings.SlmPropertyOrder = slmPropertyOrder;

            EditorUtility.SetDirty(stageLightManeuverSettings);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }

}
