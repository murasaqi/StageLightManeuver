using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    public static class SlmEditorUtility
    {
    public static void DrawDefaultGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.serializedObject.FindProperty(property.propertyPath);
        var fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        using ( new EditorGUI.PropertyScope(fieldRect, label, property)) 
        {
            if (property.hasChildren) {
                // 子要素があれば折り畳み表示
                property.isExpanded = EditorGUI.Foldout (fieldRect, property.isExpanded, label);
            }
            else {
                // 子要素が無ければラベルだけ表示
                EditorGUI.LabelField(fieldRect, label);
                return;
            }
            fieldRect.y += EditorGUIUtility.singleLineHeight;
            fieldRect.y += EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded) {

                using (new EditorGUI.IndentLevelScope()) 
                {
                    // 最初の要素を描画
                    property.NextVisible(true);
                    var depth = property.depth;
                    EditorGUI.PropertyField(fieldRect, property, true);
                    fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                    fieldRect.y += EditorGUIUtility.standardVerticalSpacing;

                    // それ以降の要素を描画
                    while(property.NextVisible(false)) {
                        
                        // depthが最初の要素と同じもののみ処理
                        if (property.depth != depth) {
                            break;
                        }
                        EditorGUI.PropertyField(fieldRect, property, true);
                        fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                        fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
        }
    }
    
    public static StageLightTimelineClip currentEditingClip;
    
    public static void OverwriteProperties(StageLightProfile stageLightProfileCopy, List<StageLightTimelineClip> selectedClips)
    {
        var properties = stageLightProfileCopy.stageLightProperties.FindAll(x => x.propertyOverride == true);

        foreach (var p in properties)
        {
            if(p.propertyOverride == false) continue;
            foreach (var selectedClip in selectedClips)
            {
                if(selectedClip.behaviour.stageLightQueData == null) continue;

                foreach (var property in selectedClip.behaviour.stageLightQueData.stageLightProperties)
                {
                    if(property == null) continue;
                    if (property.GetType() == p.GetType())
                    {
                        property.OverwriteProperty(p);
                        selectedClip.forceTimelineClipUpdate = true;
                        break;
                    }
                }
                  
            }   
        }
    }

    public static void InitAndProperties( StageLightProfile stageLightProfileCopy, List<StageLightTimelineClip> selectedClips)
    {
        stageLightProfileCopy.stageLightProperties.Clear();
        var propertyTypes = new List<System.Type>();
            
        foreach (var selectedClip in selectedClips)
        {
            foreach (var property in selectedClip.behaviour.stageLightQueData.stageLightProperties)
            {
                if(property == null) continue;
                if (propertyTypes.Contains(property.GetType())) continue;
                propertyTypes.Add(property.GetType());
            }
        }
            
        foreach (var propertyType in propertyTypes)
        {
                
            // var type = stageLightProperty.GetType();
            var slm = (Activator.CreateInstance(propertyType, BindingFlags.CreateInstance, null,
                    new object[] { }, null)
                as SlmProperty);
            // var property = System.Activator.CreateInstance(propertyType) as SlmProperty;
            stageLightProfileCopy.TryAdd(slm);
        }

        // Repaint();
    }
    public static List<StageLightTimelineClip> SelectClips()
    {
        var select = Selection.objects.ToList();
        var selectedClips = new List<StageLightTimelineClip>();
        // selectedClips.Clear();
        foreach (var s in select)
        {
            if (s.GetType().ToString() == "UnityEditor.Timeline.EditorClip")
            {
                var clip = s.GetType().GetField("m_Clip", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(s);
                    
                var timelineClip = clip as TimelineClip;
                if(timelineClip == null) continue;
                if (timelineClip.asset.GetType() == typeof(StageLightTimelineClip))
                {
                    // stringBuilder.AppendLine(timelineClip.displayName);
                    var asset = timelineClip.asset as StageLightTimelineClip;
                        
                    selectedClips.Add(asset);

                }
            }
                
        }

        return selectedClips;
        // selectedClipsField.value = stringBuilder.ToString();

    }
    public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property = property.serializedObject.FindProperty(property.propertyPath);
        var height = 0.0f;
        
        // プロパティ名
        height += EditorGUIUtility.singleLineHeight;
        height += EditorGUIUtility.standardVerticalSpacing;

        if (!property.hasChildren) {
            // 子要素が無ければラベルだけ表示
            return height;
        }
        
        if (property.isExpanded) {
        
            // 最初の要素
            property.NextVisible(true);
            var depth = property.depth;
            height += EditorGUI.GetPropertyHeight(property, true);
            height += EditorGUIUtility.standardVerticalSpacing;
            
            // それ以降の要素
            while(property.NextVisible(false))
            {
                // depthが最初の要素と同じもののみ処理
                if (property.depth != depth) {
                    break;
                }
                Debug.Log(property.name);
                height += EditorGUI.GetPropertyHeight(property, true);
                height += EditorGUIUtility.standardVerticalSpacing;
            }
            // 最後はスペース不要なので削除
            height -= EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}
}