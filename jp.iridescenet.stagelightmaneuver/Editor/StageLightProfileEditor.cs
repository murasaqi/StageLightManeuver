using System.Collections;
using System.Collections.Generic;
using StageLightManeuver;
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
            // base.OnInspectorGUI();
            stageLightProfile = (StageLightProfile) target;
            // if (GUILayout.Button("Init"))
            // {
            //     stageLightProfile.Init();
            // }
            // var stageLightPropertiesProperty= serializedObject.FindProperty("stageLightProperties");
            var stageLightPropertiesProperty = serializedObject.FindProperty("stageLightProperties");
           
               
                if (stageLightPropertiesProperty.isExpanded) {

                    using (new EditorGUI.IndentLevelScope()) 
                    {
                        // 最初の要素を描画
                        stageLightPropertiesProperty.NextVisible(true);
                        var depth = stageLightPropertiesProperty.depth;
                      
                        // それ以降の要素を描画
                        while (stageLightPropertiesProperty.NextVisible(false))
                        {

                            // depthが最初の要素と同じもののみ処理
                            if (stageLightPropertiesProperty.depth != depth)
                            {
                                break;
                            }

                            if(stageLightPropertiesProperty.managedReferenceValue == null) continue;
                            Debug.Log(stageLightPropertiesProperty.managedReferenceValue.GetType());
                            DrawStageLightProperty( stageLightPropertiesProperty);
                            // EditorGUI.PropertyField(fieldRect, property, true);
                            // fieldRect.y += EditorGUI.GetPropertyHeight(property, true);
                            // fieldRect.y += EditorGUIUtility.standardVerticalSpacing;
                        }
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
            var position = EditorGUILayout.GetControlRect();
            EditorGUI.DrawRect(position, new Color(0.3f, 0.3f, 0.3f));
            var expanded = EditorGUI.Foldout(position, serializedProperty.isExpanded, GUIContent.none);
            if (expanded != serializedProperty.isExpanded) 
            {
                serializedProperty.isExpanded = expanded; 
            } 
            position.x += 5; 
            EditorGUI.BeginChangeCheck(); 
            var isOverride = EditorGUI.ToggleLeft(position, propertyName, slmProperty.propertyOverride);
            if (EditorGUI.EndChangeCheck()) 
            { 
                slmProperty.propertyOverride = isOverride; 
                serializedObject.ApplyModifiedProperties(); 
                stageLightProfile.isUpdateGuiFlag = true; 
            } 
            EditorGUI.BeginChangeCheck();
               
               
        }
    }
}