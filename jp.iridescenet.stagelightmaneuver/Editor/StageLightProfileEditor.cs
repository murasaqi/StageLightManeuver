using System.Collections;
using System.Collections.Generic;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;
namespace StageLightManeuver
{
    [ CustomEditor ( typeof ( StageLightProfile ) , true )]
// Custom Editor stage light profile
    public class StageLightProfileEditor: Editor
    {
        public StageLightProfile stageLightProfile;
        public override void OnInspectorGUI()
        {
            stageLightProfile = (StageLightProfile) target;
            var stageLightPropertiesProperty = serializedObject.FindProperty("stageLightProperties");
            
            using (new EditorGUI.IndentLevelScope()) 
            {
                var depth = stageLightPropertiesProperty.depth;
                foreach (SerializedProperty property in stageLightPropertiesProperty)
                {
                    if (property.depth != depth+1)
                    {
                        break;
                    }
                    if(property.managedReferenceValue == null) continue;
                    DrawStageLightProperty( property);
                }
            }
            
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }


        public void DrawStageLightProperty(SerializedProperty serializedProperty)
        {
            var slmProperty = serializedProperty.managedReferenceValue as SlmProperty;
            if(slmProperty == null) return;
            var propertyName = slmProperty.propertyName;
            var expanded = false;
            if(!serializedProperty.managedReferenceValue.GetType().IsSubclassOf(typeof(SlmToggleValueBase))) return;
            expanded = DrawHeader(serializedProperty, propertyName);
            
            EditorGUI.BeginDisabledGroup(!slmProperty.propertyOverride);
            if(!expanded) return;
            var depth = serializedProperty.depth;
            foreach (SerializedProperty property in serializedProperty)
            {
                if(property.name == "propertyOverride" ||
                   property.name == "propertyName") continue;
                DrawSlmToggleValue(property);
                       
            }
            
            EditorGUI.EndDisabledGroup();


        }
        
        public void DrawSlmToggleValue(SerializedProperty serializedProperty)
        {
         
            Debug.Log(serializedProperty.propertyPath);

            if (serializedProperty.FindPropertyRelative("propertyOverride") != null)
            {
                var value = serializedProperty.FindPropertyRelative("value");
                if (value == null) return;
                var isSingleObject = true;
                isSingleObject = !value.hasChildren;
                if (isSingleObject) EditorGUILayout.BeginHorizontal();
                var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
                EditorGUI.BeginChangeCheck();
                var isOverride = EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyOverride.boolValue = isOverride;
                    serializedObject.ApplyModifiedProperties();
                    stageLightProfile.isUpdateGuiFlag = true;
                }
                if(!isSingleObject) EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!isOverride);
                if (isSingleObject)
                {
                   
                    

                    // if (value.propertyType == SerializedPropertyType.ObjectReference)
                    // {
                    //  
                    //     // DrawMinMaxEaseUI(value);
                    // }
                    // else
                    // {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(value,GUIContent.none,false);   
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }   
                    // }
                    
                }
                else
                {
                    foreach (SerializedProperty valueProperty in value)
                    {
                        if(valueProperty.name == "propertyOverride" ||
                           valueProperty.name == "propertyName") continue;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(valueProperty);   
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
                if(isSingleObject)EditorGUILayout.EndHorizontal();
                if(!isSingleObject) EditorGUI.indentLevel--;
            }
            
        }
        public bool DrawHeader(SerializedProperty serializedProperty, string propertyName, bool headerBackground = true)
        {
            // if (serializedProperty.FindPropertyRelative("propertyOverride") != null) return 
            // var slmToggleValue = serializedProperty.managedReferenceValue as SlmToggleValueBase;

            var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
            var position = EditorGUILayout.GetControlRect();
            if(headerBackground)EditorGUI.DrawRect(position, new Color(0.3f, 0.3f, 0.3f));
            var expanded = EditorGUI.Foldout(position, serializedProperty.isExpanded, GUIContent.none);
            if (expanded != serializedProperty.isExpanded) 
            {
                serializedProperty.isExpanded = expanded; 
            } 
            position.x += 5; 
            EditorGUI.BeginChangeCheck(); 
            var isOverride = EditorGUI.ToggleLeft(position, propertyName, propertyOverride.boolValue);
            if (EditorGUI.EndChangeCheck()) 
            { 
                propertyOverride.boolValue = isOverride; 
                serializedObject.ApplyModifiedProperties(); 
                stageLightProfile.isUpdateGuiFlag = true; 
            } 
            EditorGUI.BeginChangeCheck();

            return expanded;

        }
        
          protected void DrawMinMaxEaseUI(SerializedProperty serializedProperty)
        {
            
            var minMaxEasingValue = serializedProperty.managedReferenceValue as MinMaxEasingValue;
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(serializedProperty.propertyPath);
                }

                var inverse = serializedProperty.FindPropertyRelative("inverse");
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(inverse);
                    if(EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                }

                var mode = serializedProperty.FindPropertyRelative("mode");
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(mode);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                    
                }

                if (minMaxEasingValue.mode == AnimationMode.Ease)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var easeType = serializedProperty.FindPropertyRelative("easeType");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(easeType);
                        if(EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                    

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var valueMinMax = serializedProperty.FindPropertyRelative("valueMinMax");
                        EditorGUILayout.BeginHorizontal();
                        // GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new LabelWidth(60))
                            {
                                // var valueMinMax = serializedProperty.FindPropertyRelative("valueMinMax");
                                EditorGUI.BeginChangeCheck();
                                var min = EditorGUILayout.FloatField("Min",
                                    minMaxEasingValue.valueMinMax.x);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    valueMinMax.vector2Value = new Vector2(min, valueMinMax.vector2Value.y);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        GUILayout.FlexibleSpace();
                       
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new LabelWidth(60))
                            {
                                EditorGUI.BeginChangeCheck();
                                var max = EditorGUILayout.FloatField("Max",
                                    minMaxEasingValue.valueMinMax.y);
                                if (EditorGUI.EndChangeCheck())
                                {
                                   
                                    valueMinMax.vector2Value = new Vector2(valueMinMax.vector2Value.x, max);
                                    // minMaxEasingValue.GetType().GetField("valueMinMax")
                                    //     .SetValue(minMaxEasingValue,
                                    //         new Vector2(minMaxEasingValue.valueMinMax.x, max) as object);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        EditorGUILayout.FloatField(minMaxEasingValue.valueRange.x, GUILayout.Width(80));
                        EditorGUILayout.MinMaxSlider(ref minMaxEasingValue.valueRange.x,
                            ref minMaxEasingValue.valueRange.y,
                            minMaxEasingValue.valueMinMax.x, minMaxEasingValue.valueMinMax.y);
                        EditorGUILayout.FloatField(minMaxEasingValue.valueRange.y, GUILayout.Width(80));

                    }
                }
                if(minMaxEasingValue.mode == AnimationMode.AnimationCurve)
                {
                    var curve = serializedProperty.FindPropertyRelative("animationCurve");
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(curve);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                }

                if (minMaxEasingValue.mode == AnimationMode.Constant)
                {
                    var constant = serializedProperty.FindPropertyRelative("constant");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(constant);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                }
            }
        }
        
    }
}