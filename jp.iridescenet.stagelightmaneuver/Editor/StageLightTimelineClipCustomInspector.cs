using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class StageLightTimelineClipCustomInspector : UnityEditor.Editor
    {

        
        private List<StageLightPropertyEditor> _stageLightPropertyEditors = new List<StageLightPropertyEditor>();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var template = EditorGUIUtility.Load("StageLightTimelineClipEditor.uxml") as VisualTreeAsset;
            template.CloneTree(root);
            return root;
        }

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
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("referenceStageLightProfile"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
           
            var stageLightTimelineClip = serializedObject.targetObject as StageLightTimelineClip;

            if(stageLightTimelineClip == null)
                return;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                GUI.backgroundColor= Color.green;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Load Profile",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.LoadProfile();
                }
                
                
                GUI.backgroundColor= Color.white;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Save Profile",GUILayout.MaxWidth(100)))
                {
                    stageLightTimelineClip.SaveProfile();
                    
                }
                
                GUI.backgroundColor= Color.red;
                GUI.contentColor = Color.white;
                if (GUILayout.Button("Save as",GUILayout.MaxWidth(100)))
                {
                    ExportProfile(stageLightTimelineClip);
                }
                GUI.backgroundColor = Color.white;
                
            }
            
            EditorGUILayout.Space(1);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("syncReferenceProfile"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                stageLightTimelineClip.InitSyncData();
            }


            // EditorGUILayout.PropertyField(serializedObject.FindProperty("forceTimelineClipUpdate"));
            EditorGUI.BeginDisabledGroup(stageLightTimelineClip.syncReferenceProfile);
            if (stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData != null &&
                stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<LightProperty>() != null)
            {
                
            }
            var stageLightProperties = stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties;
            // var stageLightProfile = new SerializedObject( stageLightTimelineClip.stageLightQueData);
            
            _stageLightPropertyEditors.Clear();
            foreach (var property in stageLightProperties)
            {
                
                DrawStageLightPropertyGUI(property, stageLightTimelineClip);
               
            }
            
            DrawAddPropertyButton(stageLightTimelineClip);
           
            EditorGUI.EndDisabledGroup();
            // DrawPropertyInInspector(stageLightProfile.FindProperty("stageLightProperties"));
        }
        
        private void DrawAddPropertyButton(StageLightTimelineClip stageLightTimelineClip)
        {
            EditorGUI.BeginChangeCheck();

            var selectList = new List<string>()
            {
                "Add Property",
                "Light Property",
                "Pan Property",
                "Tilt Property",
                "Gobo Property",
                "Decal Property",
            };


            foreach (var property in stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData
                         .stageLightProperties)
            {
                if (selectList.Find(x => x == property.propertyName + " Property") != null)
                {
                    selectList.Remove(property.propertyName + " Property");
                }
            }
            EditorGUI.BeginDisabledGroup(selectList.Count  <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray());
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                switch (select-1)
                {
                    case 0:
                        stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(new LightProperty());
                        break;
                    case 1:
                        stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(new PanProperty());
                        break;
                    case 2:
                        stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(new TiltProperty());
                        break;
                    case 3:
                        stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(new GoboProperty());
                        break;
                    case 4:
                        stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties.Add(new DecalProperty());
                        break;
                }
            }
            
            
        }

        private void DrawRollProperty(FieldInfo[] fields)
        {
            foreach (var fieldInfo in fields)
            {
            }
        }
        

        private void DrawStageLightPropertyGUI(SlmProperty property, Object undoTarget)
        {


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
                                        .bpmOverride);
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
                                                EditorGUI.BeginChangeCheck();
                                                var bpmOverride = EditorGUILayout.Toggle("Override Time",
                                                    bpmOverrideData.bpmOverride);
                                                if (EditorGUI.EndChangeCheck())
                                                {
                                                    bpmOverrideData.GetType().GetField("bpmOverride")
                                                        .SetValue(bpmOverrideData, bpmOverride);
                                                }

                                        }

                                        EditorGUI.BeginDisabledGroup(!bpmOverrideData.bpmOverride);
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
                                            using (new LabelWidth(120))
                                            {
                                                EditorGUI.BeginChangeCheck();
                                                var bpmScaleValue = EditorGUILayout.FloatField("BPM Scale",
                                                    bpmOverrideData.bpmScale,
                                                    GUILayout.Width(180));
                                                if (EditorGUI.EndChangeCheck())
                                                {
                                                    bpmOverrideData.GetType().GetField("bpmScale")
                                                        .SetValue(bpmOverrideData, bpmScaleValue);
                                                }

                                                EditorGUI.BeginChangeCheck();
                                                var bpmOffsetValue = EditorGUILayout.FloatField("BPM Offset",
                                                    bpmOverrideData.bpmOffset,
                                                    GUILayout.Width(180));
                                                if (EditorGUI.EndChangeCheck())
                                                {
                                                    bpmOverrideData.GetType().GetField("bpmOffset")
                                                        .SetValue(bpmOverrideData, bpmOffsetValue);
                                                }
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

                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                                if (resultValue != null)
                                {
                                    stageLightValueFieldInfo.SetValue(fieldValue, resultValue);
                                }

                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUI.EndDisabledGroup();
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
                                    minMaxEasingValue.rollMinMax.x);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if(undoTarget != null)Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                                    minMaxEasingValue.GetType().GetField("rollMinMax")
                                        .SetValue(minMaxEasingValue,
                                            new Vector2(min, minMaxEasingValue.rollMinMax.y) as object);
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
                                    minMaxEasingValue.rollMinMax.y);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(undoTarget, "Changed Area Of Effect");
                                    minMaxEasingValue.GetType().GetField("rollMinMax")
                                        .SetValue(minMaxEasingValue,
                                            new Vector2(minMaxEasingValue.rollMinMax.x, max) as object);
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {

                        EditorGUILayout.FloatField(minMaxEasingValue.rollRange.x, GUILayout.Width(80));
                        EditorGUILayout.MinMaxSlider(ref minMaxEasingValue.rollRange.x,
                            ref minMaxEasingValue.rollRange.y,
                            minMaxEasingValue.rollMinMax.x, minMaxEasingValue.rollMinMax.y);
                        EditorGUILayout.FloatField(minMaxEasingValue.rollRange.y, GUILayout.Width(80));

                    }
                }else
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


        private void ExportProfile(StageLightTimelineClip stageLightTimelineClip)
        {
           
            Undo.RegisterCompleteObjectUndo(stageLightTimelineClip, stageLightTimelineClip.name);
            EditorUtility.SetDirty(stageLightTimelineClip);
            
            var exportPath = stageLightTimelineClip.referenceStageLightProfile != null ? AssetDatabase.GetAssetPath(stageLightTimelineClip.referenceStageLightProfile) : "Asset";
            var exportName = stageLightTimelineClip.referenceStageLightProfile != null ? stageLightTimelineClip.referenceStageLightProfile.name+"(Clone)" : "new stageLightProfile";
            var path = EditorUtility.SaveFilePanel("Save StageLightProfile Asset", exportPath,exportName, "asset");
            string fileName = Path.GetFileName(path);
            if(path == "") return;
            path = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            string dir = Path.GetDirectoryName(path);
            // Debug.Log($"dir: {dir}, file: {fileName}");
            var newProfile = CreateInstance<StageLightProfile>();
            newProfile.stageLightProperties = stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.stageLightProperties;
            AssetDatabase.CreateAsset(newProfile, path);
            // useProfile = true;
            // ptlPropObject = new ExposedReference<VLVLBClipProfile>();
            stageLightTimelineClip.referenceStageLightProfile = AssetDatabase.LoadAssetAtPath<StageLightProfile>(path);
            // Debug.Log($"Load {path}");
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

      

    }
}