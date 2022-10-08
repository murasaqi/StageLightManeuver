
using System;
using System.Reflection;
using StageLightManeuver;
using UnityEditor;
using UnityEngine;
// using Object = Unity.Object;

namespace StageLightManeuver.StageLightTimeline.Editor
{
    public class StageLightPropertyEditor
    {
        private SlmProperty _slmProperty;
        public StageLightPropertyEditor(SlmProperty property)
        {
            _slmProperty = property;
        }
    
        public void DrawGUI(UnityEngine.Object undoTarget)
        {
            var fields = _slmProperty.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                var _stageLightPropertyOverrideFieldInfo = _slmProperty.GetType().BaseType.GetField("_stageLightPropertyOverride");
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var enable = EditorGUILayout.Toggle((bool)_stageLightPropertyOverrideFieldInfo.GetValue(_slmProperty), GUILayout.Width(30));
                if (EditorGUI.EndChangeCheck())
                {
                    _stageLightPropertyOverrideFieldInfo.SetValue(_slmProperty,enable);
                }
                EditorGUILayout.PrefixLabel(_slmProperty.GetType().ToString());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
                
                EditorGUI.BeginDisabledGroup(!enable);
                using (new EditorGUI.IndentLevelScope())
                {
                    // いろいろ書く。この中のやつがインテントされる。
                      
                    foreach (var fieldInfo in fields)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SlmToggleValue<>))
                        {
                            var fieldValue = fieldInfo.GetValue(_slmProperty);
                            var stageLightValueFieldInfo = fieldValue.GetType().GetField("value");
                            var _stageLightPropertyOverride = fieldValue.GetType().BaseType.GetField("_stageLightPropertyOverride");
                            
                            Debug.Log(_stageLightPropertyOverride.GetValue(fieldValue));
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUI.BeginChangeCheck();
                            
                            var toggle = EditorGUILayout.Toggle((bool)_stageLightPropertyOverride.GetValue(fieldValue),
                                GUILayout.Width(40));
                            if (EditorGUI.EndChangeCheck())
                            {
                                _stageLightPropertyOverride.SetValue(fieldValue, toggle);
                            }
                            

                            EditorGUI.BeginDisabledGroup(false);
                            var displayName = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
                            var labelValue = displayName != null ? displayName.name : fieldInfo.Name;

                            object resultValue = null;
                            EditorGUI.BeginChangeCheck();
                            if (stageLightValueFieldInfo.FieldType == typeof(System.Single))
                            {
                                resultValue = EditorGUILayout.FloatField(labelValue,
                                    (float)stageLightValueFieldInfo.GetValue(fieldValue));
                               
                            }

                          if (stageLightValueFieldInfo.FieldType == typeof(System.Int32))
                            {
                                resultValue = EditorGUILayout.IntField(labelValue,
                                    (int)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(System.Boolean))
                            {
                                resultValue = EditorGUILayout.Toggle(labelValue,
                                    (bool)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(System.String))
                            {
                                resultValue = EditorGUILayout.TextField(labelValue,
                                    (string)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Color))
                            {
                                resultValue = EditorGUILayout.ColorField(labelValue,
                                    (Color)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Vector2))
                            {
                                resultValue = EditorGUILayout.Vector2Field(
                                    labelValue,(Vector2)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Vector3))
                            {
                                resultValue = EditorGUILayout.Vector3Field(
                                    labelValue,(Vector3)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Vector4))
                            {
                                resultValue = EditorGUILayout.Vector4Field(
                                    labelValue,(Vector4)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Quaternion))
                            {
                                resultValue = EditorGUILayout.Vector4Field(
                                    labelValue,(Vector4)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.AnimationCurve))
                            {
                                resultValue = EditorGUILayout.CurveField(labelValue,
                                    (AnimationCurve)stageLightValueFieldInfo.GetValue(fieldValue));
                            }
                            
                            if (stageLightValueFieldInfo.FieldType.BaseType != null && stageLightValueFieldInfo.FieldType.BaseType == typeof(System.Enum))
                            {
                                var easeType = stageLightValueFieldInfo.GetValue(fieldValue)as Enum;
                                resultValue = EditorGUILayout.EnumPopup(labelValue,easeType);
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Gradient))
                            {
                                resultValue = EditorGUILayout.GradientField(labelValue,
                                    (Gradient)stageLightValueFieldInfo.GetValue(fieldValue));
                            }
                            
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                               if(resultValue !=null) stageLightValueFieldInfo.SetValue(fieldValue, resultValue);
                            }
                            
                            
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(2);
                            EditorGUI.EndDisabledGroup();
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
        }    
        
    }
    
}