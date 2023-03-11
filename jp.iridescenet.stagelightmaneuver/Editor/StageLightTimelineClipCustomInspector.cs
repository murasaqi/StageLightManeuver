using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StageLightManeuver.StageLightTimeline.Editor
{
    public abstract class EditorGUIWidth : System.IDisposable
    {
        protected abstract void ApplyWidth(float width);
        public EditorGUIWidth(float width) { ApplyWidth(width); }
        public void Dispose() { ApplyWidth(0.0f); }
    }

    public class LabelWidth : EditorGUIWidth
    {
        public LabelWidth(float width) : base(width) { }
        protected override void ApplyWidth(float width) { EditorGUIUtility.labelWidth = width; }
    }
    
    
    [CustomEditor(typeof(StageLightTimelineClip))]
    [CanEditMultipleObjects]
    public class StageLightTimelineClipCustomInspector : UnityEditor.Editor
    {

        // private static StageLightProfile stageLightProfileCopy;
        
        private List<StageLightPropertyEditor> _stageLightPropertyEditors = new List<StageLightPropertyEditor>();
        
        private List<StageLightProfile> allProfilesInProject = new List<StageLightProfile>();
        private List<string> profileNames = new List<string>();
        private int selectedProfileIndex = 0;
        private static List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
        // group by folder
        private Dictionary<string, List<StageLightProfile>> folderNamesProfileDict = new Dictionary<string, List<StageLightProfile>>();
        

        private List<string> mExcluded = new List<string>();

        public override void OnInspectorGUI()
        {
            BeginInspector();
            mExcluded.Clear();
            // mExcluded.Add("stageLightSetting");
            // DrawRemainingPropertiesInInspector();
        }
        
        private void BeginInspector()
        {
            serializedObject.Update();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Profile", GUILayout.MaxWidth(60));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("referenceStageLightProfile"),
                    new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                // DrawProfilesPopup(serializedObject.targetObject as StageLightTimelineClip);
            }

            var stageLightTimelineClip = serializedObject.targetObject as StageLightTimelineClip;

            
            EditorGUI.BeginDisabledGroup(stageLightTimelineClip.referenceStageLightProfile == null);


            if(stageLightTimelineClip == null)
                return;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                
                GUI.backgroundColor= Color.green;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Load Profile",GUILayout.MaxWidth(100)))
                {
                    // set dirty
                    EditorUtility.SetDirty(stageLightTimelineClip);
                    stageLightTimelineClip.LoadProfile();
                    serializedObject.ApplyModifiedProperties();
                    Repaint();
                }
                
                
                GUI.backgroundColor= Color.white;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Save Profile",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.SaveProfile();
                    
                }
                
            }
            

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncReferenceProfile"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                stageLightTimelineClip.InitSyncData();
            }
            
            EditorGUI.EndDisabledGroup();


            using (new EditorGUILayout.HorizontalScope())
            {

                EditorGUI.BeginChangeCheck();
                var path = EditorGUILayout.PropertyField(serializedObject.FindProperty("exportPath"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                GUI.backgroundColor = Color.white;
                if (GUILayout.Button("...",GUILayout.MaxWidth(30)))
                {
                    SetFilePath(stageLightTimelineClip);
                }

            }

            using (new EditorGUILayout.HorizontalScope())
            {

                GUILayout.FlexibleSpace();



                GUI.backgroundColor= Color.red;
                GUI.contentColor = Color.white;
                
                if (GUILayout.Button("Save as",GUILayout.MaxWidth(100)))
                {
                    ExportProfile(stageLightTimelineClip);
                }
                
                GUI.backgroundColor = Color.white;
            }
            
            
            
            EditorGUILayout.Space(1);

           
            if (GUILayout.Button("Select StageLight",GUILayout.MaxWidth(120)))
            {
                if (stageLightTimelineClip.mixer != null && stageLightTimelineClip.mixer.trackBinding != null)
                {
                    var gameObjects = new List<GameObject>();
                    foreach (var stageLight in stageLightTimelineClip.mixer.trackBinding.AllStageLights)
                    {
                        gameObjects.Add(stageLight.gameObject);
                    }
                    Selection.objects = gameObjects.ToArray();
                }
                    
            }

            Selection.selectionChanged += () =>
            {
                selectedClips = SlmEditorUtility.SelectClips();
                SlmEditorUtility.InitAndProperties(stageLightTimelineClip.track.ReferenceStageLightProfile,selectedClips);  
                // Repaint();
            };
       
         
            var isMultiSelect = selectedClips.Count > 1;

            if (isMultiSelect)
            {

                if(selectedClips.Last() != stageLightTimelineClip)
                    return;
                

                // var serializedProfile = stageLightTimelineClip.track.SerializedProfile;
                var referenceProfile = stageLightTimelineClip.track.ReferenceStageLightProfile;
                var serializedProfile = new SerializedObject(referenceProfile);
                var stageLightPropertiesProperty = serializedProfile.FindProperty("stageLightProperties");
               
                if(stageLightPropertiesProperty == null)
                    return;
                
                for (int i = 0; i < referenceProfile.stageLightProperties.Count; i++)
                {   
                    
                    var property = referenceProfile.stageLightProperties[i];
                    if (property == null)
                    {
                        continue;
                    }
                    // Debug.Log(property.propertyName);
                    var serializedProperty = stageLightPropertiesProperty.GetArrayElementAtIndex(i);
                    if(serializedProperty == null)
                        continue;
                    StageLightProfileEditorUtil.DrawStageLightProperty(referenceProfile.stageLightProperties,serializedProperty ,false);

                    GUILayout.Space(2);
                    using (new EditorGUILayout.HorizontalScope())
                    {

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("☑ Apply checked properties", GUILayout.Width(200)))
                        {
                            SlmEditorUtility.OverwriteProperties( referenceProfile, selectedClips);
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.Space(2);
            
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(stageLightTimelineClip.syncReferenceProfile);
           
                
              
                var stageLightProperties = stageLightTimelineClip.behaviour.stageLightQueData.stageLightProperties;
                var behaviourProperty = serializedObject.FindProperty("behaviour");
                var stageLightQueDataProperty = behaviourProperty.FindPropertyRelative("stageLightQueData");
                var serializedProperty =stageLightQueDataProperty.FindPropertyRelative("stageLightProperties");
            
                // _stageLightPropertyEditors.Clear();
                for (int i = 0; i < stageLightProperties.Count; i++)
                {

                    var property = stageLightProperties[i];
                    if(property == null) continue;
                
                    var serializedSlmProperty = serializedProperty.GetArrayElementAtIndex(i);
                    var expanded = false;
                    expanded = StageLightProfileEditorUtil.DrawHeader(serializedSlmProperty, property.propertyName);
                    
                    
                    if (!expanded)
                    {
                        continue;
                    }
                    EditorGUI.BeginDisabledGroup(!property.propertyOverride);
                    
                    // get serializable property name in property
                    property.GetType().GetFields().ToList().ForEach(f =>
                    {
                        
                        StageLightProfileEditorUtil.DrawSlmToggleValue(serializedSlmProperty.FindPropertyRelative(f.Name));
                    });
                    var action = new Action(() =>
                    {
                        stageLightProperties.Remove(property);
                        return;
                    });
                    StageLightProfileEditorUtil.DrawRemoveButton(serializedObject,stageLightProperties, action);
                    
                    EditorGUI.EndDisabledGroup();
                }
            
                DrawAddPropertyButton(stageLightTimelineClip);
           
                EditorGUI.EndDisabledGroup();    
            }
            
        }
        
        private void DrawAddPropertyButton(StageLightTimelineClip stageLightTimelineClip)
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
            foreach (var property in stageLightTimelineClip.behaviour.stageLightQueData
                         .stageLightProperties)
            {
               if(property == null) continue;
                if (selectList.Find(x => x== property.GetType().Name) != null)
                {
                    selectList.Remove(property.GetType().Name);
                }
                    
                
            }
            
            EditorGUI.BeginDisabledGroup(selectList.Count  <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray());
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                // SetDirty 
                Undo.RecordObject(stageLightTimelineClip, "Add Property");
                EditorUtility.SetDirty(stageLightTimelineClip);   
                var type = SlmUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;

                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightTimelineClip.behaviour.stageLightQueData.TryGet<LightProperty>();
                    var lightIntensityProperty = stageLightTimelineClip.behaviour.stageLightQueData.TryGet<LightIntensityProperty>();
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
                stageLightTimelineClip.behaviour.stageLightQueData.stageLightProperties.Add(property);
                
                // apply serialized object
                serializedObject.ApplyModifiedProperties();
                //Save asset
                AssetDatabase.SaveAssets();
            }
            
            
        }

        private void DrawRollProperty(FieldInfo[] fields)
        {
            foreach (var fieldInfo in fields)
            {
            }
        }
        

        private void DrawStageLightPropertyGUI(SlmProperty property, Object undoTarget, Action onRemove)
        {

            if(property == null) return;

            using (new EditorGUILayout.VerticalScope("GroupBox"))
            {
                var fields = property.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToList();
                var propertyOverrideFieldInfo = property.GetType().BaseType.GetField("propertyOverride");
                var isPropertyOverride = (bool)propertyOverrideFieldInfo.GetValue(property);
               
                var opened =EditorGUILayout.Foldout( isPropertyOverride, property.propertyName);

                if(opened != isPropertyOverride)
                    propertyOverrideFieldInfo.SetValue(property,opened);
                
                if (!opened)
                {
                    return;
                }
                
                EditorGUI.indentLevel++;
               

                var orderedFields = new List<FieldInfo>();
                // var bpmOverrideFields = new List<FieldInfo>();
                if (property.GetType().BaseType == typeof(SlmAdditionalProperty) || property.GetType().BaseType == typeof(RollProperty))
                {
                    orderedFields.Add(fields.Find(x=>x.Name == "bpmOverrideData"));
                 
                    foreach (var f in fields)
                    {
                        if(f.Name != "bpmOverrideData")
                            orderedFields.Add(f);
                    }
                }
                else
                {
                    orderedFields = fields;
                }
                

                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var fieldInfo in orderedFields)
                    {
                        var fieldValue = fieldInfo.GetValue(property);
                        var fieldType = fieldInfo.FieldType;
                        var displayName = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
                        var labelValue = displayName != null ? displayName.name : fieldInfo.Name;

                        if (fieldType == typeof(ClipProperty))
                        {

                            var clipProperty = fieldValue as ClipProperty;
                            EditorGUILayout.BeginHorizontal();
                            // GUILayout.FlexibleSpace();      
                            EditorGUILayout.LabelField(
                                $"Constant Duration: {clipProperty.clipEndTime - clipProperty.clipStartTime}");
                            // EditorGUILayout.LabelField($"End: {clipProperty.clipEndTime}");

                            EditorGUILayout.EndHorizontal();


                        }
                        else if (fieldType.IsGenericType &&
                                 fieldType.GetGenericTypeDefinition() == typeof(SlmToggleValue<>))
                        {
                            object resultValue = null;
                            var stageLightValueFieldInfo = fieldValue.GetType().GetField("value");
                            var propertyOverride = fieldValue.GetType().BaseType.GetField("propertyOverride");
                            EditorGUILayout.BeginHorizontal();

                            EditorGUI.BeginChangeCheck();

                            var propertyOverrideToggle = false;

                            propertyOverrideToggle = EditorGUILayout.Toggle((bool)propertyOverride.GetValue(fieldValue),
                                GUILayout.Width(40));

                            if (EditorGUI.EndChangeCheck())
                            {
                                propertyOverride.SetValue(fieldValue, propertyOverrideToggle);
                            }

                            EditorGUI.BeginDisabledGroup(!propertyOverrideToggle);


                            EditorGUI.BeginChangeCheck();
                            if (stageLightValueFieldInfo.FieldType == typeof(System.Single))
                            {

                                if (property.GetType().BaseType == typeof(SlmAdditionalProperty) &&
                                    labelValue == "BPM Scale" ||
                                    property.GetType().BaseType == typeof(SlmAdditionalProperty) &&
                                    labelValue == "BPM Offset")
                                {
                                    var stageLightAdditionalProperty = property as SlmAdditionalProperty;
                                    EditorGUI.BeginDisabledGroup(!stageLightAdditionalProperty.bpmOverrideData.value
                                        .propertyOverride);
                                    using (new EditorGUI.IndentLevelScope())
                                    {
                                        resultValue = EditorGUILayout.FloatField(labelValue,
                                            (float)stageLightValueFieldInfo.GetValue(fieldValue));
                                    }

                                    EditorGUI.EndDisabledGroup();
                                }
                                else
                                {
                                    resultValue = EditorGUILayout.FloatField(labelValue,
                                        (float)stageLightValueFieldInfo.GetValue(fieldValue));
                                }
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(MinMaxEasingValue))
                            {
                                DrawMinMaxEaseUI(labelValue, stageLightValueFieldInfo, fieldValue, undoTarget);
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(BpmOverrideToggleValueBase))
                            {
                                var bpmOverrideData =
                                    stageLightValueFieldInfo.GetValue(fieldValue) as BpmOverrideToggleValueBase;
                                // Debug.Log(bpmOverrideData);

                                using (new EditorGUILayout.VerticalScope())
                                {

                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        EditorGUILayout.LabelField("BPM Override");   
                                    }
                                    using (new EditorGUI.IndentLevelScope())
                                    {
                                        using (new EditorGUILayout.HorizontalScope())
                                        {

                                            // using (new LabelWidth(120))
                                            // {
                                                // EditorGUI.BeginChangeCheck();
                                                // var bpmOverride = EditorGUILayout.Toggle("Override Time",
                                                //     bpmOverrideData.propertyOverride);
                                                // if (EditorGUI.EndChangeCheck())
                                                // {
                                                //     bpmOverrideData.GetType().GetField("bpmOverride")
                                                //         .SetValue(bpmOverrideData, bpmOverride);
                                                // }

                                        }

                                        EditorGUI.BeginDisabledGroup(!bpmOverrideData.propertyOverride);
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            // using (new EditorCommon.LabelWidth(100))
                                            // {
                                            EditorGUI.BeginChangeCheck();
                                            var resultLoopType =
                                                EditorGUILayout.EnumPopup("Loop Type", bpmOverrideData.loopType);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                bpmOverrideData.GetType().GetField("loopType")
                                                    .SetValue(bpmOverrideData, resultLoopType);
                                            }
                                            // }
                                        }
                                        
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            EditorGUI.BeginChangeCheck();
                                            var bpmScaleValue = EditorGUILayout.FloatField("Offset Time",
                                                bpmOverrideData.offsetTime);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                bpmOverrideData.GetType().GetField("offsetTime")
                                                    .SetValue(bpmOverrideData, bpmScaleValue);
                                            }
                                            
                                        }


                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            EditorGUI.BeginChangeCheck();
                                            var bpmScaleValue = EditorGUILayout.FloatField("BPM Scale",
                                                bpmOverrideData.bpmScale);
                                            
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                if (bpmScaleValue == 0)
                                                {
                                                    bpmScaleValue = 0.0001f;
                                                }
                                                bpmOverrideData.GetType().GetField("bpmScale")
                                                    .SetValue(bpmOverrideData, bpmScaleValue);
                                            }
                                        }

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            EditorGUI.BeginChangeCheck();
                                            var bpmOffsetValue = EditorGUILayout.FloatField("Child Stagger",
                                                bpmOverrideData.childStagger);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                bpmOverrideData.GetType().GetField("childStagger")
                                                    .SetValue(bpmOverrideData, bpmOffsetValue);
                                            }
                                     
                                        }

                                        EditorGUI.EndDisabledGroup();
                                    }
                                }
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
                                    labelValue, (Vector2)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Vector3))
                            {
                                resultValue = EditorGUILayout.Vector3Field(
                                    labelValue, (Vector3)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Vector4))
                            {
                                resultValue = EditorGUILayout.Vector4Field(
                                    labelValue, (Vector4)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Quaternion))
                            {
                                resultValue = EditorGUILayout.Vector4Field(
                                    labelValue, (Vector4)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.AnimationCurve))
                            {
                                resultValue = EditorGUILayout.CurveField(labelValue,
                                    (AnimationCurve)stageLightValueFieldInfo.GetValue(fieldValue));

                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(Texture2D))
                            {
                                resultValue = EditorGUILayout.ObjectField(labelValue,
                                    (Texture2D)stageLightValueFieldInfo.GetValue(fieldValue), typeof(Texture2D), false);
                            }
                            
                            if (stageLightValueFieldInfo.FieldType == typeof(Texture))
                            {
                                resultValue = EditorGUILayout.ObjectField(labelValue,
                                    (Texture)stageLightValueFieldInfo.GetValue(fieldValue), typeof(Texture), false);
                            }

                            if (stageLightValueFieldInfo.FieldType.BaseType != null &&
                                stageLightValueFieldInfo.FieldType.BaseType == typeof(System.Enum))
                            {
                                var easeType = stageLightValueFieldInfo.GetValue(fieldValue) as Enum;
                                resultValue = EditorGUILayout.EnumPopup(labelValue, easeType);
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Gradient))
                            {
                                resultValue = EditorGUILayout.GradientField(labelValue,
                                    (Gradient)stageLightValueFieldInfo.GetValue(fieldValue));
                            }
                            if (stageLightValueFieldInfo.FieldType == typeof(UnityEngine.Color))
                            {
                                resultValue = EditorGUILayout.ColorField(labelValue,
                                    (Color)stageLightValueFieldInfo.GetValue(fieldValue));
                            }

                            if (stageLightValueFieldInfo.FieldType == typeof(List<PanTiltPrimitive>))
                            {
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        
                                        EditorGUILayout.LabelField("Pan Tilt List");
                                    }
                                    
                                    // var rect = EditorGUI.RectField()
                                    var panTiltList = stageLightValueFieldInfo.GetValue(fieldValue) as List<PanTiltPrimitive>;
                                   
                                    for (int k = 0; k< panTiltList.Count; k++)
                                    {
                                        var panTilt = panTiltList[k];

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUI.color = Color.gray;
                                            EditorGUILayout.LabelField(panTilt.name);
                                            GUI.color = Color.white;
                                        }
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            

                                            using (new LabelWidth(60))
                                            {
                                                var pan = EditorGUILayout.FloatField("Pan", panTilt.pan);
                                                var tilt =
                                                    EditorGUILayout.FloatField("Tilt", panTilt.tilt);
                                                panTiltList[k].pan = pan;
                                                panTiltList[k].tilt = tilt;
                                            }

                                        }
                                    }
                                    
                                    resultValue = panTiltList;
                                
                                
                                }
                            }
                            
                            if (stageLightValueFieldInfo.FieldType == typeof(List<LightPrimitiveValue>))
                            {
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        
                                        EditorGUILayout.LabelField("Light List");
                                    }
                                    
                                    // var rect = EditorGUI.RectField()
                                    var lightPrimitiveValues = stageLightValueFieldInfo.GetValue(fieldValue) as List<LightPrimitiveValue>;
                                   
                                    for (int k = 0; k< lightPrimitiveValues.Count; k++)
                                    {
                                        var lightPrimitive = lightPrimitiveValues[k];

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUI.color = Color.gray;
                                            EditorGUILayout.LabelField(lightPrimitive.name);
                                            GUI.color = Color.white;
                                        }
                                        // using (new EditorGUILayout.HorizontalScope())
                                        // {
                                            
                                            var intensity = EditorGUILayout.FloatField("Intensity", lightPrimitive.intensity);
                                            lightPrimitiveValues[k].intensity = intensity;

                                            var angle = EditorGUILayout.FloatField("Angle", lightPrimitive.angle);
                                            lightPrimitiveValues[k].angle = angle;
                                            
                                            var innerAngle = EditorGUILayout.FloatField("Inner Angle", lightPrimitive.innerAngle);
                                            lightPrimitiveValues[k].innerAngle = innerAngle;
                                            
                                            var range = EditorGUILayout.FloatField("Range", lightPrimitive.range);
                                            lightPrimitiveValues[k].range = range;
                                            
                                        // }
                                    }
                                    
                                    resultValue = lightPrimitiveValues;
                                }
                            }
                            
                            if (stageLightValueFieldInfo.FieldType == typeof(List<ColorPrimitiveValue>))
                            {
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        
                                        EditorGUILayout.LabelField("Color List");
                                    }
                                    
                                    // var rect = EditorGUI.RectField()
                                    var colorPrimitiveValues = stageLightValueFieldInfo.GetValue(fieldValue) as List<ColorPrimitiveValue>;
                                   
                                    for (int k = 0; k< colorPrimitiveValues.Count; k++)
                                    {
                                        var colorPrimitiveValue = colorPrimitiveValues[k];

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUI.color = Color.gray;
                                            EditorGUILayout.LabelField(colorPrimitiveValue.name);
                                            GUI.color = Color.white;
                                        }
                                        // using (new EditorGUILayout.HorizontalScope())
                                        // {
                                            
                                        
                                            
                                            var color = EditorGUILayout.ColorField("color", colorPrimitiveValue.color);
                                            colorPrimitiveValues[k].color = color;
                                            
                                        // }
                                    }
                                    
                                    resultValue = colorPrimitiveValues;
                                }
                            }
                           
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(undoTarget, "Changed Area Of Effect");

                                if (property.GetType().BaseType == typeof(SlmAdditionalProperty) &&
                                    labelValue == "BPM Scale" ||
                                    labelValue == "BPM")
                                {
                                    var bpmScale = (float) resultValue;
                                    if (bpmScale == 0f)
                                    {
                                        resultValue = 0.0001f;
                                    }
                                }
                                if (resultValue != null)
                                {
                                    stageLightValueFieldInfo.SetValue(fieldValue, resultValue);
                                }

                            }
                            

                            EditorGUILayout.EndHorizontal();
                            EditorGUI.EndDisabledGroup();
                        }
                    }

                    using (new EditorGUILayout.VerticalScope())
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
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }

        protected void DrawMinMaxEaseUI(string labelName ,FieldInfo fieldInfo,object target, Object undoTarget=null)
        {
            
            var minMaxEasingValue = fieldInfo.GetValue(target) as MinMaxEasingValue;
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(labelName);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    var inverse = minMaxEasingValue.inverse;
                    EditorGUI.BeginChangeCheck();
                    var resultBoole = EditorGUILayout.Toggle("Inverse", inverse);
                    if(EditorGUI.EndChangeCheck())
                    {
                        minMaxEasingValue.GetType().GetField("inverse").SetValue(minMaxEasingValue,resultBoole);
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    // using (new EditorCommon.LabelWidth(80))
                    // {
                        var animationMode = EditorGUILayout.EnumPopup("Mode", minMaxEasingValue.mode);
                    if (EditorGUI.EndChangeCheck())
                    {
                        minMaxEasingValue.GetType().GetField("mode")
                            .SetValue(minMaxEasingValue, animationMode);
                    }
                    // }   
                }

                if (minMaxEasingValue.mode == AnimationMode.Ease)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var easeType = minMaxEasingValue.easeType;
                        EditorGUI.BeginChangeCheck();
                        var resultEaseType = EditorGUILayout.EnumPopup("Ease Type", easeType);
                        if(EditorGUI.EndChangeCheck())
                        {
                            minMaxEasingValue.GetType().GetField("easeType").SetValue(minMaxEasingValue,resultEaseType);
                        }
                    }
                    

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.BeginHorizontal();
                        // GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new LabelWidth(60))
                            {
                                EditorGUI.BeginChangeCheck();
                                var min = EditorGUILayout.FloatField("Min",
                                    minMaxEasingValue.minMaxValue.x);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if(undoTarget != null)Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                                    minMaxEasingValue.GetType().GetField("valueMinMax")
                                        .SetValue(minMaxEasingValue,
                                            new Vector2(min, minMaxEasingValue.minMaxValue.y) as object);
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
                                    minMaxEasingValue.minMaxValue.y);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                                    minMaxEasingValue.GetType().GetField("valueMinMax")
                                        .SetValue(minMaxEasingValue,
                                            new Vector2(minMaxEasingValue.minMaxValue.x, max) as object);
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        EditorGUILayout.FloatField(minMaxEasingValue.minMaxLimit.x, GUILayout.Width(80));
                        EditorGUILayout.MinMaxSlider(ref minMaxEasingValue.minMaxLimit.x,
                            ref minMaxEasingValue.minMaxLimit.y,
                            minMaxEasingValue.minMaxValue.x, minMaxEasingValue.minMaxValue.y);
                        EditorGUILayout.FloatField(minMaxEasingValue.minMaxLimit.y, GUILayout.Width(80));

                    }
                }
                if(minMaxEasingValue.mode == AnimationMode.AnimationCurve)
                {
                    EditorGUI.BeginChangeCheck();
                    var curveResult = EditorGUILayout.CurveField("Curve",
                        (AnimationCurve)minMaxEasingValue.animationCurve);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                        minMaxEasingValue.GetType().GetField("animationCurve")
                            .SetValue(minMaxEasingValue,
                                curveResult);
                    }
                }

                if (minMaxEasingValue.mode == AnimationMode.Constant)
                {
                    EditorGUI.BeginChangeCheck();
                    var floatResult = EditorGUILayout.FloatField("constant",
                        minMaxEasingValue.constant);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                        minMaxEasingValue.GetType().GetField("constant")
                            .SetValue(minMaxEasingValue,
                                floatResult);
                    }
                }
            }
        }
        
        protected void DrawRemainingPropertiesInInspector()
        {
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, mExcluded.ToArray());
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
        
        protected void DrawPropertyInInspector(SerializedProperty p)
        {
            if (!IsPropertyExcluded(p.name))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(p);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                ExcludeProperty(p.name);
            }
        }
        
        private bool IsPropertyExcluded(string propertyName)
        {
            return mExcluded.Contains(propertyName);
        }

        private void ExcludeProperty(string propertyName)
        {
            mExcluded.Add(propertyName);
        }

        private void SetFilePath(StageLightTimelineClip stageLightTimelineClip)
        {
            var exportPath = stageLightTimelineClip.referenceStageLightProfile != null ? AssetDatabase.GetAssetPath(stageLightTimelineClip.referenceStageLightProfile) : "Asset";
            var exportName = stageLightTimelineClip.referenceStageLightProfile != null ? stageLightTimelineClip.referenceStageLightProfile.name+"(Clone)" : "new stageLightProfile";
            var path = EditorUtility.SaveFilePanel("Save StageLightProfile Asset", exportPath,exportName, "asset");
            string fileName = Path.GetFileName(path);
            if(path == "") return;
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            string dir = Path.GetDirectoryName(path);
            stageLightTimelineClip.exportPath = path;

            serializedObject.ApplyModifiedProperties();
        }


        private void ExportProfile(StageLightTimelineClip stageLightTimelineClip)
        {
           
            Undo.RegisterCompleteObjectUndo(stageLightTimelineClip, stageLightTimelineClip.name);
            EditorUtility.SetDirty(stageLightTimelineClip);
            
            var newProfile = CreateInstance<StageLightProfile>();
            newProfile.stageLightProperties = stageLightTimelineClip.behaviour.stageLightQueData.stageLightProperties;
            var exportPath = SlmUtility.GetExportPath(stageLightTimelineClip.exportPath,stageLightTimelineClip.clipDisplayName) + ".asset";

            // if directory not exist, create it
            var directory = Path.GetDirectoryName(exportPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            
            // if same name file exist in directory, add (number) to file name
            var fileName = Path.GetFileNameWithoutExtension(exportPath);
            var fileExtension = Path.GetExtension(exportPath);
            var filePath = Path.GetDirectoryName(exportPath);
            
            // try .asset file 
            var files = Directory.GetFiles(filePath, "*" + fileExtension).ToList().Where( f => f.Contains(fileName)).ToList();
            var fileNames = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
            // sort file names
            fileNames.Sort();
            
            // fileNames.ForEach(f => Debug.Log(f));
            var lastFileNumber = 0;
            var exportFileName = fileName;
            if (fileNames.Count > 0)
            {
                var lastFile = fileNames.Last();
                var match = Regex.Match(lastFile, @"\((\d+)\)$");
                if (match.Success)
                {
                    lastFileNumber = int.TryParse (match.Groups[1].Value, out lastFileNumber) ? lastFileNumber : 0;
                }

                fileName = fileName.Replace($"({lastFileNumber})", "");
                lastFileNumber++;
            }

            if (lastFileNumber == 0)
            {
                exportPath = filePath + "/" + fileName + fileExtension;
            }
            else
            {
                exportPath = filePath + "/" + fileName+ $"({lastFileNumber})" + fileExtension;
            }
            
                




            AssetDatabase.CreateAsset(newProfile, exportPath);
            AssetDatabase.Refresh();
            InitProfileList(stageLightTimelineClip);
            stageLightTimelineClip.referenceStageLightProfile = AssetDatabase.LoadAssetAtPath<StageLightProfile>(exportPath);
            // EditorUtility.SetDirty(stageLightTimelineClip);
            AssetDatabase.SaveAssets();
            // serializedObject.Applyy(stageLightTimelineClip);
            // AssetDatabase.SaveAssets();
            // serializedObject.ApplyModifiedProperties();
            //
        }
        
        
        private void OnDisable()
        {
           
        }

        private void OnDestroy()
        {
        }
        public void OnInspectorUpdate()
        {
            this.Repaint();
        }
        
        private void InitProfileList(StageLightTimelineClip stageLightTimelineClip)
        {
            allProfilesInProject = SlmUtility.GetProfileInProject();
            profileNames.Clear();

            // group by folder
            folderNamesProfileDict = new Dictionary<string, List<StageLightProfile>>();
            foreach (var profile in allProfilesInProject)
            {
                var path = AssetDatabase.GetAssetPath(profile);
                var parentDirectory = Path.GetDirectoryName(path).Replace("Assets/", "").Replace("Assets\\", "");
                parentDirectory = parentDirectory.Replace("\\", ">").Replace("/", ">");
                if (folderNamesProfileDict.ContainsKey(parentDirectory))
                {
                    folderNamesProfileDict[parentDirectory].Add(profile);
                }
                else
                {
                    folderNamesProfileDict.Add(parentDirectory, new List<StageLightProfile> {profile});
                }

            }

            foreach (var keyPair in folderNamesProfileDict)
            {
                foreach (var v in keyPair.Value)
                {
                    profileNames.Add($"{keyPair.Key}/{v.name}");
                }
            }
            
            selectedProfileIndex = allProfilesInProject.IndexOf(stageLightTimelineClip.referenceStageLightProfile);
        }
        
        private void DrawProfilesPopup(StageLightTimelineClip stageLightTimelineClip)
        {
            
            if(allProfilesInProject == null || allProfilesInProject.Count == 0)
                InitProfileList(stageLightTimelineClip);
            
            EditorGUI.BeginChangeCheck();
            selectedProfileIndex = EditorGUILayout.Popup("", selectedProfileIndex, profileNames.ToArray(), GUILayout.Width(120));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(stageLightTimelineClip, "Changed StageLightProfile");
                stageLightTimelineClip.referenceStageLightProfile = allProfilesInProject[selectedProfileIndex];
                serializedObject.ApplyModifiedProperties();
            }
        }

        
        


    }
}