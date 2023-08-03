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
        private ReorderableList _reorderableSlmProperties;
        private bool _isExpandedReorderableProperties = false;
        private List<Type> _slmPropertyTypes;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            stageLightManeuverSettings = target as StageLightManeuverSettings;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("exportProfilePath"));
            // Draw line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (_reorderableSlmProperties == null)
            {
                var slmProperties = stageLightManeuverSettings.SlmPropertyOrder
                                        .OrderBy(x => x.Value)
                                        .ToDictionary(x => x.Key, x => x.Value).Keys
                                        .ToList();
                slmProperties.Remove(typeof(ClockProperty));
                slmProperties.Remove(typeof(StageLightOrderProperty));

                _reorderableSlmProperties = new ReorderableList(slmProperties, typeof(Type), true, true, false, false);
                // _reorderableSlmProperties.multiSelect = true;
                _reorderableSlmProperties.drawHeaderCallback = (rect) =>
                {
                    rect.x += 10;
                    _isExpandedReorderableProperties = EditorGUI.Foldout(rect, _isExpandedReorderableProperties, "SlmProperty Order");
                };
                _reorderableSlmProperties.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = _reorderableSlmProperties.list[index];
                    // 型名を表示
                    EditorGUI.LabelField(rect, element.ToString().Split('.').Last());
                };
            }

            if (_isExpandedReorderableProperties)
            {
                _reorderableSlmProperties.DoLayoutList();
            }
            else
            {
                // exec private method DoListHeader() from ReorderableList by reflection
                var rect = EditorGUILayout.GetControlRect();
                var method = _reorderableSlmProperties.GetType().GetMethod("DoListHeader", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(_reorderableSlmProperties, new object[] { rect });
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
                //     serializedObject.ApplyModifiedProperties();
                //     UpdatePropertyOrder();
                //     AssetDatabase.SaveAssets();
                //     AssetDatabase.Refresh();
                //     Repaint();
                //     // Debug.Log("[StageLightManeuverSettings] Reset to default");
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
            for (var i = 0; i < _reorderableSlmProperties.list.Count; i++)
            {
                var type = _reorderableSlmProperties.list[i] as Type;
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
