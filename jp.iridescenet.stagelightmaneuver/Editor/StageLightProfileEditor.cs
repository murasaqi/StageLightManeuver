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
            if(!expanded) return;
            var depth = serializedProperty.depth;
            foreach (SerializedProperty property in serializedProperty)
            {
                if(property.name == "propertyOverride" ||
                   property.name == "propertyName") continue;
                DrawSlmToggleValue(property);
                       
            }


        }
        public void DrawSlmToggleValue(SerializedProperty serializedProperty)
        {

            if (serializedProperty.FindPropertyRelative("propertyOverride") != null)
            {
                var value = serializedProperty.FindPropertyRelative("value");
                if (value == null) return;
                var isSingleObject = true;
                isSingleObject = !value.hasChildren;
                if (isSingleObject) EditorGUILayout.BeginHorizontal();
                var propertyOverride = serializedProperty.FindPropertyRelative("propertyOverride");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.ToggleLeft(serializedProperty.displayName, propertyOverride.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    stageLightProfile.isUpdateGuiFlag = true;
                }
                if(!isSingleObject) EditorGUI.indentLevel++;

                if (isSingleObject)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(value,GUIContent.none,false);   
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        stageLightProfile.isUpdateGuiFlag = true;
                    }
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
               
                
                
                
                if(isSingleObject)EditorGUILayout.EndHorizontal();
                if(!isSingleObject) EditorGUI.indentLevel--;
            }
                // serializedProperty.get
                // foreach (SerializedProperty property in serializedProperty)
                // {
                // }
                // if(!isExpanded) return;

                // var value = serializedProperty.FindPropertyRelative("value");
                // if (value == null) return;
                // EditorGUI.BeginChangeCheck();
                //
                // EditorGUILayout.PropertyField(value);
                // if (EditorGUI.EndChangeCheck())
                // {
                //     serializedObject.ApplyModifiedProperties();
                //     stageLightProfile.isUpdateGuiFlag = true;
                // }
            // }

            // var expanded = false;
            // if(!serializedProperty.managedReferenceValue.GetType().IsSubclassOf(typeof(SlmToggleValueBase))) return;
            // expanded = DrawHeader(serializedProperty, serializedProperty.displayName);
            // if(!expanded) return;


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
    }
}