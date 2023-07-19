using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// StageLightPropertyの基底Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(SlmProperty), true)]
    public class SlmPropertyDrawer : SlmTogglePropertyDrawer
    {
        public SlmPropertyDrawer() : base(true) { }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get SlmProperty from SerializedObject
            var slmProperty = property.GetValue<object>() as SlmProperty;
            if (slmProperty == null) return;
            label.text = slmProperty.propertyName;

            //  Draw header
            DrawHeader(position, property, label);
            if (property.isExpanded == false) return;

            var propertyOverride = slmProperty.propertyOverride;
            EditorGUI.BeginDisabledGroup(propertyOverride == false);
            DrawToggleController(slmProperty);

            var fields = slmProperty.GetType().GetFields().ToList();
            var clockOverride = fields.Find(x => x.FieldType == typeof(SlmToggleValue<ClockOverride>));
            if (clockOverride != null)
            {
                fields.Remove(clockOverride);
                fields.Insert(0, clockOverride);
            }
            fields = RemoveHiddenField(fields);

            EditorGUI.indentLevel++;
            fields.ForEach(f =>
            {
                // Draw SlmToggleValue
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(property.FindPropertyRelative(f.Name));
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            });
            EditorGUI.indentLevel--;

            GUILayout.Space(4);
            EditorGUI.EndDisabledGroup();
        }


        protected void DrawHeader(Rect position, SerializedProperty property, GUIContent label) => base.OnGUI(position, property, label);

        protected static void DrawToggleController(SlmProperty slmProperty)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle();
                style.normal.background = null;
                style.fixedWidth = 40;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.gray;
                // GUILayout.FlexibleSpace();
                if (GUILayout.Button("All", style))
                {
                    slmProperty.ToggleOverride(true);
                }

                GUILayout.Space(2);
                if (GUILayout.Button("None", style))
                {
                    slmProperty.ToggleOverride(false);
                    slmProperty.propertyOverride = true;
                }
            }
        }
        protected static List<FieldInfo> RemoveHiddenField(List<FieldInfo> fields)
        {
            var hiddenFields = fields.FindAll(x => x.Name == "propertyType" ||
                                                        x.Name == "propertyName" ||
                                                        x.Name == "propertyOrder" ||
                                                        x.Name == "propertyOverride" ||
                                                        x.Name == "sortOrder");
            if (hiddenFields != null)
            {
                hiddenFields.ForEach(x => fields.Remove(x));
            }
            return fields;
        }
    }
}
