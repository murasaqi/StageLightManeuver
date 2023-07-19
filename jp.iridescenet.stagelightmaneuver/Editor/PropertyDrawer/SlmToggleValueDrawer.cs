using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(SlmToggleValue<>), true)]
    public class SlmToggleValueDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawSlmToggleValue(property);
        }

        /// <summary>
        /// SlmToggleValue の共通描画処理
        /// </summary>
        /// <param name="property"></param>
        /// <param name="marginBottom"></param>
        protected static void DrawSlmToggleValue(SerializedProperty property, int marginBottom = 0)
        {
            if (property == null) return;

            var propertyOverride = property.FindPropertyRelative("propertyOverride");
            if (propertyOverride != null)
            {
                SerializedProperty value = property.FindPropertyRelative("value");
                if (value == null) return;
                var valueObject = value.GetValue<object>();
                if (valueObject == null) return;

                if (valueObject.GetType() == typeof(SlmToggleValue<ClockProperty>))
                {
                    var slmToggleValue = valueObject as SlmToggleValue<ClockProperty>;
                    slmToggleValue.sortOrder = -999;
                    property.serializedObject.ApplyModifiedProperties();
                }

                var hasMultiLineObject = IsVerticalLayoutField(valueObject);
                if (!hasMultiLineObject) EditorGUILayout.BeginHorizontal();

                // propertyOverride = property.FindPropertyRelative("propertyOverride");
                EditorGUI.BeginChangeCheck();
                var isOverride = EditorGUILayout.ToggleLeft(property.displayName, propertyOverride.boolValue, GUILayout.Width(160));
                if (EditorGUI.EndChangeCheck())
                {
                    propertyOverride.boolValue = isOverride;
                    property.serializedObject.ApplyModifiedProperties();
                    // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                }

                if (hasMultiLineObject) EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!isOverride);

                if (valueObject.GetType().BaseType == typeof(SlmProperty))
                {
                    foreach (SerializedProperty childProperty in value)
                    {
                        if (childProperty.name == "propertyOverride" ||
                            childProperty.name == "propertyName") continue;
                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.PropertyField(childProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(value, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUI.EndDisabledGroup();
                if (!hasMultiLineObject) EditorGUILayout.EndHorizontal();
                // EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1));

                GUILayout.Space(marginBottom);
                if (hasMultiLineObject) EditorGUI.indentLevel--;
            }
            else
            {
                var serializedObject = property.GetValue<object>();
                if (serializedObject == null) return;
                if (serializedObject.GetType() == typeof(ArrayStaggerValue) ||
                    serializedObject.GetType() == typeof(StageLightOrderQueue))
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
        }

        protected static bool IsVerticalLayoutField(object value)
        {
            var hasVerticalLayoutType = (value.GetType() == typeof(MinMaxEasingValue) ||
                    value.GetType() == typeof(ClockOverride) ||
                    value.GetType().IsArray || value.GetType().IsGenericType);
            return hasVerticalLayoutType;
        }
    }
}