using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    /// <summary>
    /// <see cref="SlmProperty"/>の配列を描画するプロパティドロワー
    /// </summary>
    // [CustomPropertyDrawer(typeof(List<SlmProperty>))]
    public class StageLightPropertiesDrawer : SlmBaseDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stageLightProperties = property.GetValue<object>() as List<SlmProperty>;
            stageLightProperties.RemoveAll(x => x == null);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));
            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                if (slmProperty == null) continue;
                // if (i >= property.arraySize)
                // {
                //     return;
                // }

                var serializedSlmProperty = property.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedSlmProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedSlmProperty.serializedObject.ApplyModifiedProperties();
                }

                if (serializedSlmProperty.isExpanded)
                {
                    var action = new Action(() =>
                    {
                        stageLightProperties.Remove(slmProperty);
                        return;
                    });
                    if (slmProperty.GetType() != typeof(ClockProperty))
                    {
                        DrawRemoveButton(property.serializedObject, stageLightProperties, action);
                        GUILayout.Space(SlmDrawerConst.Spacing);
                    }
                }
            }

            DrawAddPropertyButton(property.serializedObject, stageLightProperties);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;
            var stageLightProperties = property.GetValue<object>() as List<SlmProperty>;
            stageLightProperties.RemoveAll(x => x == null);

            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                if (slmProperty == null) continue;
                height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                height += EditorGUIUtility.singleLineHeight;
            }
            height += EditorGUIUtility.singleLineHeight;
            return height;
        }

        private static void DrawRemoveButton(SerializedObject serializedObject, List<SlmProperty> properties, Action onRemove)
        {
            GUILayout.Space(SlmDrawerConst.Spacing);
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
            GUILayout.Space(SlmDrawerConst.Spacing);
        }

        private void DrawAddPropertyButton(SerializedObject serializedObject, List<SlmProperty> stageLightProperties)
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();

            SlmEditorUtility.SlmPropertyTypes.ForEach(t =>
            {
                if (t != typeof(RollProperty)) selectList.Add(t.Name);
            });

            selectList.Insert(0, "Add Property");
            foreach (var property in stageLightProperties)
            {
                if (property == null) continue;
                if (selectList.Find(x => x == property.GetType().Name) != null)
                {
                    selectList.Remove(property.GetType().Name);
                }
            }
            EditorGUI.BeginDisabledGroup(selectList.Count <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.MinWidth(200));
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                var type = SlmEditorUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;

                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightProperties.Find(x => x.GetType() == typeof(LightProperty)) as LightProperty;
                    var lightIntensityProperty = stageLightProperties.Find(x => x.GetType() == typeof(LightIntensityProperty)) as LightIntensityProperty;
                    if (lightProperty != null)
                    {
                        manualLightArrayProperty.initialValue.angle = lightProperty.spotAngle.value.constant;
                        manualLightArrayProperty.initialValue.innerAngle = lightProperty.innerSpotAngle.value.constant;
                        manualLightArrayProperty.initialValue.range = lightProperty.range.value.constant;
                    }
                    if (lightIntensityProperty != null)
                    {
                        manualLightArrayProperty.initialValue.intensity = lightIntensityProperty.lightToggleIntensity.value.constant;
                    }
                }
                stageLightProperties.Add(property);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }


    // [CustomPropertyDrawer(typeof(StageLightQueueData))]
    // public class StageLightQueueDataDrawer : SlmBaseDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         var stageLightPropertys = property.FindPropertyRelative("stageLightProperties");
    //         var drawerType = GetPropertyDrawerTypeForType(typeof(List<SlmProperty>));
    //         if (drawerType != null)
    //         {
    //             var drawer = Activator.CreateInstance(drawerType) as PropertyDrawer;
    //             drawer.OnGUI(position, stageLightPropertys, label);
    //         }
    //     }

    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         return SlmDrawerConst.NoMarginHeight;
    //     }
    // }
}