using System;
using System.Collections.Generic;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    public static class StageLightProfileEditorUtil
    {
        public static void DrawStageLightProperty(SerializedObject serializedObject, SerializedProperty serializedProperty, int index)
        {
            var stageLightProfile = serializedObject.targetObject as StageLightProfile;
            var slmProperty = serializedProperty.managedReferenceValue as SlmProperty;
            if(slmProperty == null) return;
            var propertyName = slmProperty.propertyName;
            var expanded = false;
            if(!serializedProperty.managedReferenceValue.GetType().IsSubclassOf(typeof(SlmToggleValueBase))) return;
            expanded = DrawHeader(serializedObject,serializedProperty, propertyName);
            
            if(!expanded) return;
                EditorGUI.BeginDisabledGroup(!slmProperty.propertyOverride);

            var depth = serializedProperty.depth;
            foreach (SerializedProperty property in serializedProperty)
            {
                if(property.name == "propertyOverride" ||
                   property.name == "propertyName") continue;
                
                // Get custom attribute of serialized property
                DrawSlmToggleValue(serializedObject, property);

            }
            var action = new Action(() =>
            {
                stageLightProfile.stageLightProperties.Remove(slmProperty);
                return;
            });

            DrawRemoveButton(serializedObject, stageLightProfile.stageLightProperties, action);
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
            return (value.GetType() != typeof(MinMaxEasingValue) &&
             value.GetType().BaseType != typeof(SlmToggleValueBase) &&
             !value.GetType().IsArray && !value.GetType().IsGenericType);
        }
        public static void DrawSlmToggleValue(SerializedObject serializedObject,SerializedProperty serializedProperty)
        {
            
            var stageLightProfile = serializedObject.targetObject as StageLightProfile;
         
            // Debug.Log(serializedProperty.propertyPath);

            if (serializedProperty.FindPropertyRelative("propertyOverride") != null)
            {
                SerializedProperty value = serializedProperty.FindPropertyRelative("value");
                if (value == null) return;
                var valueObject = value.GetValue<object>();
                if(valueObject == null) return;
                var isSingleObject = IsVerticalLayoutField(valueObject);
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

                if (!isSingleObject) EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!isOverride);
                if (valueObject.GetType() == typeof(MinMaxEasingValue))
                {
                    DrawMinMaxEaseUI(serializedObject,value);
                }
                else if (valueObject.GetType().BaseType == typeof(SlmToggleValueBase))
                {

                    foreach (SerializedProperty childProperty in value)
                    {
                        if (childProperty.name == "propertyOverride" ||
                            childProperty.name == "propertyName") continue;
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(childProperty);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(value, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                }
                EditorGUI.EndDisabledGroup();
                if(isSingleObject)EditorGUILayout.EndHorizontal();
                if(!isSingleObject) EditorGUI.indentLevel--;
            }
            
        }
        public static bool DrawHeader(SerializedObject serializedObject, SerializedProperty serializedProperty, string propertyName, bool headerBackground = true)
        {
            var stageLightProfile = serializedObject.targetObject as StageLightProfile;
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
        
        public static void DrawMinMaxEaseUI(SerializedObject serializedObject,SerializedProperty serializedProperty)
        {
            
            var stageLightProfile = serializedObject.targetObject as StageLightProfile;
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    // EditorGUILayout.LabelField(serializedProperty.propertyPath);
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
                
                var minMaxLimitProperty = serializedProperty.FindPropertyRelative("minMaxLimit");
                var minMaxValueProperty = serializedProperty.FindPropertyRelative("minMaxValue");
                var minMaxValue = minMaxValueProperty.vector2Value;
                var minMaxLimit = minMaxLimitProperty.vector2Value;

                if (minMaxValue.x == minMaxValue.y)
                {
                
                    minMaxLimitProperty.vector2Value = new Vector2(minMaxValue.y - 1f, minMaxValue.y);
                    serializedObject.ApplyModifiedProperties();
                    stageLightProfile.isUpdateGuiFlag = true;
                }

                if (minMaxLimit.x > minMaxValue.x)
                {
                    minMaxLimitProperty.vector2Value = new Vector2(minMaxValue.x, minMaxLimit.y);
                    serializedObject.ApplyModifiedProperties();
                    stageLightProfile.isUpdateGuiFlag = true;
                }
                
                if (minMaxLimit.y < minMaxValue.y)
                {
                    minMaxLimitProperty.vector2Value = new Vector2(minMaxLimit.x, minMaxValue.y);
                    serializedObject.ApplyModifiedProperties();
                    stageLightProfile.isUpdateGuiFlag = true;
                }
                
                if (mode.enumValueIndex == 0)
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
                        EditorGUILayout.BeginHorizontal();
                        // GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new LabelWidth(90))
                            {
                                EditorGUI.BeginChangeCheck();
                                var min = EditorGUILayout.FloatField("Min Limit",
                                    minMaxLimitProperty.vector2Value.x);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    minMaxLimitProperty.vector2Value = new Vector2(min, minMaxLimitProperty.vector2Value.y);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new LabelWidth(90))
                            {
                                EditorGUI.BeginChangeCheck();
                                var max = EditorGUILayout.FloatField("Max Limit",
                                    minMaxLimitProperty.vector2Value.y);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    minMaxLimitProperty.vector2Value = new Vector2(minMaxLimitProperty.vector2Value.x, max);
                                    serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                
                        EditorGUILayout.EndHorizontal();
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {

                        var minValue = minMaxValueProperty.vector2Value.x;
                        var maxValue = minMaxValueProperty.vector2Value.y;

                        EditorGUI.BeginChangeCheck();
                        var x = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.x, GUILayout.Width(80));
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(x, minMaxValueProperty.vector2Value.y);
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.MinMaxSlider(ref minValue,
                            ref maxValue,
                            minMaxLimitProperty.vector2Value.x, minMaxLimitProperty.vector2Value.y);
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(minValue, maxValue);
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
                        }
                        
                        EditorGUI.BeginChangeCheck();
                        var y = EditorGUILayout.FloatField(minMaxValueProperty.vector2Value.y, GUILayout.Width(80));
                        if (EditorGUI.EndChangeCheck())
                        {
                            minMaxValueProperty.vector2Value = new Vector2(x, y);
                            serializedObject.ApplyModifiedProperties();
                            stageLightProfile.isUpdateGuiFlag = true;
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
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
                }
                
                if (mode.enumValueIndex == 2)
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
          
        private static void DrawAddPropertyButton(SerializedObject serializedObject, StageLightProfile stageLightProfile)
        {
            EditorGUI.BeginChangeCheck();


            // var propertyTypes = SlmUtility.GetTypes(typeof(SlmAdditionalProperty));

            // propertyTypes.Remove(typeof(RollProperty));
            var selectList = new List<string>();
            
            SlmUtility.SlmAdditionalTypes.ForEach(t =>
            {
                if(t != typeof(RollProperty))selectList.Add(t.Name);
            });
            
            
            
            // var typeDict = new Dictionary<string, Type>();
            
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
                // SetDirty 
                EditorUtility.SetDirty(stageLightProfile);   
                var type = SlmUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmAdditionalProperty;

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