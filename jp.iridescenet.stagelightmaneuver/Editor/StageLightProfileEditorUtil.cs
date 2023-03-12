using System;
using System.Collections.Generic;
using System.Linq;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLightManeuver
{
    public static class StageLightProfileEditorUtil
    {
       

        
        public static void DrawStageLightProperty(List<SlmProperty> stageLightProperties, SerializedProperty serializedProperty, bool drawRemoveButton)
        {
            // var stageLightProperties = stageLightProfile.stageLightProperties;
            var serializedObject = serializedProperty.serializedObject;
            var slmProperty = serializedProperty.GetValue<SlmProperty>();
            if(slmProperty == null) return;
            var propertyName = slmProperty.propertyName;
            var expanded = false;
            if(!slmProperty.GetType().IsSubclassOf(typeof(SlmToggleValueBase))) return;
            expanded = DrawHeader(serializedProperty, propertyName);

            if (!expanded)
            {
                return;
            }
            
         
            EditorGUI.BeginDisabledGroup(!slmProperty.propertyOverride);

            
               
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle();
                style.normal.background =null;
                style.fixedWidth = 40;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.gray;
                // GUILayout.FlexibleSpace();
                if (GUILayout.Button("All", style))
                {
                    slmProperty.ToggleOverride(true);
                    // stageLightProfile.isUpdateGuiFlag = true;
                }
                
                GUILayout.Space(2);
                if (GUILayout.Button("None", style))
                {
                    slmProperty.ToggleOverride(false);
                    slmProperty.propertyOverride = true;
                    // stageLightProfile.isUpdateGuiFlag = true;
                }
                    
            }
            
            
            foreach (SerializedProperty property in serializedProperty)
            {
                if(property.name == "propertyOverride" ||
                   property.name == "propertyName") continue;
                DrawSlmToggleValue(property);

            }


            if (drawRemoveButton)
            {
                var action = new Action(() =>
                {
                    stageLightProperties.Remove(slmProperty);
                    return;
                });
                DrawRemoveButton(serializedObject, stageLightProperties,action);
            }
            EditorGUI.EndDisabledGroup();
            
          
        }

       

        public static void DrawRemoveButton(SerializedObject serializedObject, List<SlmProperty> properties, Action onRemove)
        {
            GUILayout.Space(2);
            using (new EditorGUILayout.HorizontalScope())
            {

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(120)))
                {
                    onRemove?.Invoke();
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(2);
      

        }

        public static bool IsVerticalLayoutField(object value)
        {
            var hasVerticalLayoutType = (value.GetType() == typeof(MinMaxEasingValue) ||
                    value.GetType() == typeof(ClockOverride) ||
                    value.GetType().IsArray || value.GetType().IsGenericType);
            return hasVerticalLayoutType;
        }


        public static void DrawOneLineSlmToggleValue(SerializedProperty serializedProperty,int marginBottom = 0)
        {
           
            var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
            if(propertyOverride == null) return;
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var isOverride = EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue, GUILayout.Width(120));
            if (EditorGUI.EndChangeCheck())
            {
                propertyOverride.boolValue = isOverride;
                serializedProperty.serializedObject.ApplyModifiedProperties();
                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("value"), GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                serializedProperty.serializedObject.ApplyModifiedProperties();
                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        
        public static void DrawSlmToggleValue(SerializedProperty serializedProperty, int marginBottom = 0)
        {
            if(serializedProperty == null) return;
            
            if (serializedProperty.FindPropertyRelative("propertyOverride") != null)
            {
                SerializedProperty value = serializedProperty.FindPropertyRelative("value");
                if (value == null) return;
                var valueObject = value.GetValue<object>();
                if(valueObject == null) return;

                if (valueObject.GetType() == typeof(SlmToggleValue<ClockOverride>))
                {
                    var slmToggleValue = valueObject as SlmToggleValue<ClockOverride>;
                    slmToggleValue.sortOrder = -999;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }
                
                var hasMultiLineObject = IsVerticalLayoutField(valueObject);
                if (!hasMultiLineObject) EditorGUILayout.BeginHorizontal();
             
                var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
                EditorGUI.BeginChangeCheck();
                var isOverride = EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue,GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck())
                {
                    propertyOverride.boolValue = isOverride;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                }

                if (hasMultiLineObject) EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!isOverride);


                if (valueObject.GetType() == typeof(MinMaxEasingValue))
                {
                    DrawMinMaxEaseUI(value);
                }else if (valueObject.GetType() == typeof(ClockOverride))
                {
                    var loopType = value.FindPropertyRelative("loopType");
                    var childDepth = value.depth+1;
                    while(value.NextVisible(true) && value.depth >= childDepth){
                        if (value.depth == childDepth)
                        {
                            if (value.name == "arrayStaggerValue" && loopType.enumValueIndex == 3)
                            {
                                var serializedObject = value.GetValue<object>();
                                ArrayStaggerValue(value, serializedObject as ArrayStaggerValue);
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(value);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    serializedProperty.serializedObject.ApplyModifiedProperties();
                                    // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                                }
                            }
                        }
                    }
                }
                else if (valueObject.GetType().BaseType == typeof(SlmProperty))
                {

                    foreach (SerializedProperty childProperty in value)
                    {
                        if (childProperty.name == "propertyOverride" ||
                            childProperty.name == "propertyName") continue;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(childProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedProperty.serializedObject.ApplyModifiedProperties();
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
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                    }
                }
                EditorGUI.EndDisabledGroup();
                if(!hasMultiLineObject)EditorGUILayout.EndHorizontal();
                // EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1));
                
                GUILayout.Space(marginBottom);
                if(hasMultiLineObject) EditorGUI.indentLevel--;
                
            }
            else
            {
                var serializedObject = serializedProperty.GetValue<object>();
                if(serializedObject.GetType() == typeof(ArrayStaggerValue))
                {
                    ArrayStaggerValue(serializedProperty, serializedObject as ArrayStaggerValue);
                }
            }
            
            
           
        }

        public static void DrawStaggerMinMaxSliders(ArrayStaggerValue arrayStaggerValue, SerializedProperty serializedProperty)
        {
         
            var expand =EditorGUILayout.Foldout(serializedProperty.isExpanded, serializedProperty.displayName);
            if (expand != serializedProperty.isExpanded)
            {
                serializedProperty.isExpanded = expand;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            if (!expand)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            EditorGUI.indentLevel++;
            var arrayValue = serializedProperty.GetValue<object>() as List<Vector2>;
            if (arrayValue == null) return;


            foreach (var value in arrayValue)
            {
                var min = value.x;
                var max = value.y;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.MinMaxSlider(ref min,ref max,0,1f);
                if (EditorGUI.EndChangeCheck())
                {
                    var index = arrayValue.IndexOf(value);
                    serializedProperty.GetArrayElementAtIndex(index).vector2Value = new Vector2(min,max);
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }
                            
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static void ArrayStaggerValue(SerializedProperty serializedProperty, ArrayStaggerValue arrayStaggerValue)
        {
            var animationDurationProperty = serializedProperty.FindPropertyRelative("animationDuration");
            var childDepth = serializedProperty.depth+1;
            while(serializedProperty.NextVisible(true) && serializedProperty.depth >= childDepth){
                if (serializedProperty.depth == childDepth)
                {

                    if (serializedProperty.name == "lightStaggerInfo" || serializedProperty.name == "randomStaggerInfo")
                    {
                        
                      
                        if (arrayStaggerValue.staggerCalculationType == StaggerCalculationType.Random &&
                            serializedProperty.name == "randomStaggerInfo")
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if(GUILayout.Button("Set Random",GUILayout.Width(100)))
                                {
                                    arrayStaggerValue.CalculateRandomStaggerTime();
                                }
                                GUILayout.FlexibleSpace();
                            }
                           

                            DrawStaggerMinMaxSliders( arrayStaggerValue, serializedProperty);
                            
                            EditorGUILayout.EndFoldoutHeaderGroup();
                        }
                        if( arrayStaggerValue.staggerCalculationType != StaggerCalculationType.Random &&
                            serializedProperty.name == "lightStaggerInfo")
                        {
                            DrawStaggerMinMaxSliders( arrayStaggerValue, serializedProperty);
                        }
                        
                        
                        
                    }else if (serializedProperty.name == "staggerCalculationType")
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(serializedProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                        }     
                    }
                   
                }
            }
            
        }
        public static bool DrawHeader(SerializedProperty serializedProperty, string propertyName, bool headerBackground = true)
        {
            // var stageLightProfile = serializedObject.targetObject as StageLightProfile;
            if (serializedProperty == null) return false;
            var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
            if(propertyOverride == null) return false;
            var position = EditorGUILayout.GetControlRect();
            if(headerBackground)EditorGUI.DrawRect(position, new Color(0.3f, 0.3f, 0.3f));
            var expanded = EditorGUI.Foldout(position, serializedProperty.isExpanded, GUIContent.none);
            if (expanded != serializedProperty.isExpanded) 
            {
                serializedProperty.isExpanded = expanded; 
                serializedProperty.serializedObject.ApplyModifiedProperties();
                // stageLightProfile.isUpdateGuiFlag = true;
            } 
            position.x += 5; 
            // EditorGUI.BeginChangeCheck(); 
            // EditorGUI.showMixedValue = true;
            // {
            var isOverride = EditorGUI.ToggleLeft(position, propertyName, propertyOverride.boolValue);
            
            if (propertyOverride.boolValue != isOverride) 
            { 
                propertyOverride.boolValue = isOverride; 
                serializedProperty.serializedObject.ApplyModifiedProperties();
                // stageLightProfile.isUpdateGuiFlag = true;
                // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true; 
            } 
            // EditorGUI.BeginChangeCheck(); 

            return expanded;

        }
        
        public static void DrawMinMaxEaseUI(SerializedProperty serializedProperty)
        {
            
            // var stageLightProfile = serializedObject.targetObject as StageLightProfile;
            using (new EditorGUILayout.VerticalScope())
            {
                var inverse = serializedProperty.FindPropertyRelative("inverse");
                var mode = serializedProperty.FindPropertyRelative("mode");
                var currentPosition = EditorGUILayout.GetControlRect();


                currentPosition.width = 80;
                    EditorGUI.LabelField(currentPosition,"Inverse");
                    EditorGUI.BeginChangeCheck();
                    currentPosition.x += 50;
                    currentPosition.width = 20;
                    EditorGUI.PropertyField(currentPosition,inverse,GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                    }
               
                    currentPosition.x += 30;
                    currentPosition.width = 80;
                    EditorGUI.LabelField(currentPosition,"Mode");
                    currentPosition.x += 40;
                    currentPosition.width = 200;
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(currentPosition,mode,GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                    }

                    // GUILayout.FlexibleSpace();
                

                EditorGUI.indentLevel++;
                
                var minMaxLimitProperty = serializedProperty.FindPropertyRelative("minMaxLimit");
                var minMaxValueProperty = serializedProperty.FindPropertyRelative("minMaxValue");
                var minMaxValue = minMaxValueProperty.vector2Value;
                var minMaxLimit = minMaxLimitProperty.vector2Value;
                
                

                
                if (mode.enumValueIndex == 0)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var easeType = serializedProperty.FindPropertyRelative("easeType");
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(easeType);
                        if(EditorGUI.EndChangeCheck())
                        {
                            serializedProperty.serializedObject.ApplyModifiedProperties();
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
                                    min = min >= minMaxLimit.y ? minMaxLimit.y-1 : min;
                                    minMaxLimitProperty.vector2Value = new Vector2(min, minMaxLimitProperty.vector2Value.y);
                                    serializedProperty.serializedObject.ApplyModifiedProperties();
                                    
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
                                    max = max <= minMaxLimit.x ? minMaxLimit.x+1 : max;
                                    minMaxLimitProperty.vector2Value = new Vector2(minMaxLimitProperty.vector2Value.x, max);
                                    serializedProperty.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                
                        EditorGUILayout.EndHorizontal();
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {

                        var minValue = minMaxValueProperty.vector2Value.x;
                        var maxValue = minMaxValueProperty.vector2Value.y;

                        if(minMaxLimit.x > minMaxValueProperty.vector2Value.x)
                        {
                            minMaxValueProperty.vector2Value = new Vector2(minMaxLimit.x, minMaxValueProperty.vector2Value.y);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                        }
                        
                        if(minMaxLimit.y < minMaxValueProperty.vector2Value.y)
                        {
                            minMaxValueProperty.vector2Value = new Vector2(minMaxValueProperty.vector2Value.x, minMaxLimit.y);
                            serializedProperty.serializedObject.ApplyModifiedProperties();  
                        }
                        
                        EditorGUI.BeginChangeCheck();
                        var x = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.x, GUILayout.Width(80));
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(x, minMaxValueProperty.vector2Value.y);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.MinMaxSlider(ref minValue,
                            ref maxValue,
                            minMaxLimitProperty.vector2Value.x, minMaxLimitProperty.vector2Value.y);
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(minValue, maxValue);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                        }
                        
                        EditorGUI.BeginChangeCheck();
                        var y = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.y, GUILayout.Width(80));
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(x, y);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            // if(stageLightProfile) stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
                if(mode.enumValueIndex == 1)
                {
                    var curve = serializedProperty.FindPropertyRelative("animationCurve");
                    EditorGUI.BeginChangeCheck();
                
                    EditorGUILayout.PropertyField(curve);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        // stageLightProfile.isUpdateGuiFlag = true;
                    }
                }
                
                if (mode.enumValueIndex == 2)
                {
                    var constant = serializedProperty.FindPropertyRelative("constant");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(constant);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        // if(stageLightProfile)stageLightProfile.isUpdateGuiFlag = true;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

      
        
          
        private static void DrawAddPropertyButton(SerializedObject serializedObject, StageLightProfile stageLightProfile)
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();
            SlmUtility.SlmPropertyTypes.ForEach(t =>
            {
                if(t != typeof(RollProperty))selectList.Add(t.Name);
            });
            
            selectList.Insert(0,"Add Property");
            foreach (var property in stageLightProfile.stageLightProperties)
            {
               if(property == null) continue;
                if (selectList.Find(x => x== property.GetType().Name) != null)
                {
                    selectList.Remove(property.GetType().Name);
                }
            }
            EditorGUI.BeginDisabledGroup(selectList.Count  <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.MinWidth(200));
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(stageLightProfile);   
                var type = SlmUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;

                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightProfile.TryGet<LightProperty>();
                    var lightIntensityProperty = stageLightProfile.TryGet<LightIntensityProperty>();
                    if(lightProperty != null)
                    {
                        manualLightArrayProperty.initialValue.angle = lightProperty.spotAngle.value.constant;
                        manualLightArrayProperty.initialValue.innerAngle= lightProperty.innerSpotAngle.value.constant;
                        manualLightArrayProperty.initialValue.range = lightProperty.range.value.constant;
                    }
                    
                    if(lightIntensityProperty != null)
                    {
                        manualLightArrayProperty.initialValue.intensity = lightIntensityProperty.lightToggleIntensity.value.constant;
                    }
                }
                stageLightProfile.stageLightProperties.Add(property);
                //Save asset
                AssetDatabase.SaveAssets();
                // apply serialized object
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.Refresh();
               
            }
            
            
        }

    }
}