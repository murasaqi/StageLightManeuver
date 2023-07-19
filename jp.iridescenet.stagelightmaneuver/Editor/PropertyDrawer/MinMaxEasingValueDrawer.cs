using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(MinMaxEasingValue))]
    public class MinMaxEasingValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                var inverse = property.FindPropertyRelative("inverse");
                var mode = property.FindPropertyRelative("mode");

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(inverse);
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();

                }


                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(mode);
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel++;

                var minMaxLimitProperty = property.FindPropertyRelative("minMaxLimit");
                var minMaxValueProperty = property.FindPropertyRelative("minMaxValue");
                var minMaxValue = minMaxValueProperty.vector2Value;
                var minMaxLimit = minMaxLimitProperty.vector2Value;



                if (mode.propertyType == SerializedPropertyType.Enum)
                {
                    if (mode.enumValueIndex == 0)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            var easeType = property.FindPropertyRelative("easeType");
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(easeType);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.serializedObject.ApplyModifiedProperties();
                                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                            }
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.BeginHorizontal();
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                using (new LabelWidth(110))
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var min = EditorGUILayout.FloatField("Min Limit",
                                        minMaxLimitProperty.vector2Value.x);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        min = min >= minMaxLimit.y ? minMaxLimit.y - 1 : min;
                                        minMaxLimitProperty.vector2Value =
                                            new Vector2(min, minMaxLimitProperty.vector2Value.y);
                                        property.serializedObject.ApplyModifiedProperties();

                                        // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                                    }
                                }
                            }

                            GUILayout.FlexibleSpace();
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                using (new LabelWidth(110))
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var max = EditorGUILayout.FloatField("Max Limit",
                                        minMaxLimitProperty.vector2Value.y);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        max = max <= minMaxLimit.x ? minMaxLimit.x + 1 : max;
                                        minMaxLimitProperty.vector2Value =
                                            new Vector2(minMaxLimitProperty.vector2Value.x, max);
                                        property.serializedObject.ApplyModifiedProperties();
                                    }
                                }
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {

                            var minValue = minMaxValueProperty.vector2Value.x;
                            var maxValue = minMaxValueProperty.vector2Value.y;

                            if (minMaxLimit.x > minMaxValueProperty.vector2Value.x)
                            {
                                minMaxValueProperty.vector2Value =
                                    new Vector2(minMaxLimit.x, minMaxValueProperty.vector2Value.y);
                                property.serializedObject.ApplyModifiedProperties();
                            }

                            if (minMaxLimit.y < minMaxValueProperty.vector2Value.y)
                            {
                                minMaxValueProperty.vector2Value =
                                    new Vector2(minMaxValueProperty.vector2Value.x, minMaxLimit.y);
                                property.serializedObject.ApplyModifiedProperties();
                            }

                            EditorGUI.BeginChangeCheck();
                            var x = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.x, GUILayout.Width(80));
                            if (EditorGUI.EndChangeCheck())
                            {
                                minMaxValueProperty.vector2Value = new Vector2(x, minMaxValueProperty.vector2Value.y);
                                property.serializedObject.ApplyModifiedProperties();
                                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                            }

                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.MinMaxSlider(ref minValue,
                                ref maxValue,
                                minMaxLimitProperty.vector2Value.x, minMaxLimitProperty.vector2Value.y);
                            if (EditorGUI.EndChangeCheck())
                            {
                                minMaxValueProperty.vector2Value = new Vector2(minValue, maxValue);
                                property.serializedObject.ApplyModifiedProperties();
                                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                            }

                            EditorGUI.BeginChangeCheck();
                            var y = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.y, GUILayout.Width(80));
                            if (EditorGUI.EndChangeCheck())
                            {
                                minMaxValueProperty.vector2Value = new Vector2(x, y);
                                property.serializedObject.ApplyModifiedProperties();
                                // if(stageLightProfile) stageLightProfile.isUpdateGuiFlag = true;
                            }
                        }
                    }

                    if (mode.enumValueIndex == 1)
                    {
                        var curve = property.FindPropertyRelative("animationCurve");
                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.PropertyField(curve);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            // stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }

                    if (mode.enumValueIndex == 2)
                    {
                        var constant = property.FindPropertyRelative("constant");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(constant);
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }
    }
}