// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// using System.Reflection;
// using System.Linq;
//
// namespace StageLightManeuver
// {
//     [CustomPropertyDrawer(typeof(SlmToggleValue<>), true)]
//   
//     
//     
//     public class SlmToggleValueDrawer : PropertyDrawer
//     {
//         
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             var valueProperty = property.FindPropertyRelative("value");
//             var propertyOverride = property.FindPropertyRelative("propertyOverride");
//             var height = EditorGUI.GetPropertyHeight(propertyOverride, label, true);
//             
//             height +=  EditorGUI.GetPropertyHeight( valueProperty, label, true);
//             
//             return height;
//         }
//         // Draw the property inside the given rect
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             
//             
//        
//             
//             // Draw label
//             // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//
//             // Don't make child fields be indented
//             var indent = EditorGUI.indentLevel;
//             EditorGUI.indentLevel = 0;
//
//             // Calculate rects
//             var toggleButtonWidth = 20;
//             var margin = 10;
//             var valueProperty = property.FindPropertyRelative("value");
//             var propertyOverride = property.FindPropertyRelative("propertyOverride");
//             // Draw fields - passs GUIContent.none to each so they are drawn without labels
//             EditorGUI.BeginChangeCheck();
//             // get single hight
//             var singleHeight = EditorGUI.GetPropertyHeight(propertyOverride, new GUIContent(label.text), true);
//             EditorGUI.ToggleLeft( new Rect(position.x,position.y,position.width,singleHeight), label,propertyOverride.boolValue);
//             if (EditorGUI.EndChangeCheck())
//             {
//                 property.serializedObject.ApplyModifiedProperties();
//             }
//             position.y += EditorGUI.GetPropertyHeight(propertyOverride, label, true);
//             EditorGUI.BeginChangeCheck();
//             EditorGUI.PropertyField(position, valueProperty, GUIContent.none, true);
//             if (EditorGUI.EndChangeCheck())
//             {
//                 property.serializedObject.ApplyModifiedProperties();
//             }
//             
//
//
//             // Set indent back to what it was
//             EditorGUI.indentLevel = indent;
//
//             
//         }
//     }
// }