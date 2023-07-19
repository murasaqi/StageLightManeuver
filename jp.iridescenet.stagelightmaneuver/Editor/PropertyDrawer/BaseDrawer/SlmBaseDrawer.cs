using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// プロパティーの名前とPositionのみを保持する規定Drawer
    /// </summary> 
    public class SlmBaseDrawer : PropertyDrawer
    {
        /// <summary>
        /// プロパティーの表示名
        /// <see cref="GetDisplayName(GUIContent)"/>で初期化される
        /// </summary>
        public string DiaplayName { get; protected set; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = GetDisplayName(label);
            EditorGUI.LabelField(position, label);
        }

        /// <summary>
        /// <see cref="DiaplayName"/>を初期化する
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public GUIContent GetDisplayName(GUIContent label)
        {
            if (label == GUIContent.none)
            {
                DiaplayName = label.text;
                return label;
            }
            var labelCopy = new GUIContent(label);

            // DisplayNameAttribute がある場合は、そちらのnameプロパティーを使用する
            var displayNameAttribute = fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).OfType<DisplayNameAttribute>().ToList().FirstOrDefault();
            if (displayNameAttribute != null && displayNameAttribute.name != null)
            {
                labelCopy.text = displayNameAttribute.name;
            }
            DiaplayName = labelCopy.text;
            return labelCopy;
        }

        // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        // {
        //     return EditorGUI.GetPropertyHeight(property);
        // }

        // public Rect CalcPosition(SerializedProperty property)
        // {
        //     var position = EditorGUILayout.GetControlRect();
        //     // var height = EditorGUI.GetPropertyHeight(property);
        //     return CalcPosition(position, property);
        // }

        // public Rect CalcPosition(Rect position, SerializedProperty property)
        // {
        //     position.y -= EditorGUI.GetPropertyHeight(property, true);
        //     // position.height = height;
        //     return position;
        // }
    }
}
