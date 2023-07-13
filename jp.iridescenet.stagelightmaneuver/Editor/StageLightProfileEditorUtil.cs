using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                style.fixedWidth = 60;
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
                var marginBottom =slmProperty.GetType() == typeof(ClockProperty) ? 0 : 4;
                Debug.Log(slmProperty.GetType());
                if (slmProperty.GetType() == typeof(ClockProperty))
                {
                    var clockProperty = slmProperty as ClockProperty;
                    var loopType = clockProperty.loopType.value;

                    if (loopType == LoopType.FixedStagger)
                    {
                        if (property.name == "arrayStaggerValue" || property.name == "loopType")
                        {
                            DrawSlmToggleValue(property,marginBottom);
                        }
                    }
                    else
                    {
                        if (property.name != "arrayStaggerValue")
                        {
                            DrawSlmToggleValue(property,marginBottom);
                        }
                    }
                }
                else
                {
                    DrawSlmToggleValue(property,marginBottom);
                }
                

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
                // GetCustomAttributes
             
                if(valueObject == null) return;

                if (valueObject.GetType() == typeof(SlmToggleValue<ClockProperty>))
                {
                    var slmToggleValue = valueObject as SlmToggleValue<ClockProperty>;
                    slmToggleValue.sortOrder = -999;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                }

                // if (valueObject.GetType() == typeof(SlmToggleValue<StageLightOrderProperty>))
                // {
                //     var slmToggleValue = valueObject as SlmToggleValue<StageLightOrderProperty>;
                //     slmToggleValue.sortOrder = -998;
                //     serializedProperty.serializedObject.ApplyModifiedProperties();
                // }

                var hasMultiLineObject = IsVerticalLayoutField(valueObject);
                if (!hasMultiLineObject) EditorGUILayout.BeginHorizontal();
             
                var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
                EditorGUI.BeginChangeCheck();
                var isOverride = EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue,GUILayout.Width(160));
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
                    EditorGUILayout.PropertyField(value, GUIContent.none);
                }
                else if (valueObject.GetType() == typeof(ArrayStaggerValue))
                {
                    EditorGUILayout.PropertyField(value, GUIContent.none);
                }
                else if (valueObject.GetType() == typeof(ClockOverride))
                {
                    EditorGUILayout.PropertyField(value, GUIContent.none);
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
                if(serializedObject == null) return;
                if(serializedObject.GetType() == typeof(ArrayStaggerValue))
                {
                    EditorGUILayout.PropertyField(serializedProperty);
                }
                else if (serializedObject.GetType() == typeof(StageLightOrderQueue))
                {
                    var stageLightOrderQueue = serializedObject as StageLightOrderQueue;
                    var settingListName = new List<string>();
                    if(stageLightOrderQueue == null) return;
                    settingListName.Add("(0) None");
                    var stageLightOrderSettingList = stageLightOrderQueue.stageLightOrderSettingList;
                    foreach (var stageLightOrderSetting in stageLightOrderSettingList)
                    {
                        var dropDownIndex = stageLightOrderSettingList.IndexOf(stageLightOrderSetting)+1;
                        settingListName.Add($"({dropDownIndex}) {stageLightOrderSetting.name}");
                    }
                    EditorGUI.BeginChangeCheck();
                    var index = EditorGUILayout.Popup( "Settings", stageLightOrderQueue.index+1, settingListName.ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        stageLightOrderQueue.index = index-1;
                        Debug.Log(stageLightOrderQueue.index);
                        serializedProperty.serializedObject.ApplyModifiedProperties();
                        
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
        
          
        private static void DrawAddPropertyButton(SerializedObject serializedObject, StageLightProfile stageLightProfile)
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();
            SlmEditorUtility.SlmPropertyTypes.ForEach(t =>
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
                var type = SlmEditorUtility.GetTypeByClassName(selectList[select]);
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