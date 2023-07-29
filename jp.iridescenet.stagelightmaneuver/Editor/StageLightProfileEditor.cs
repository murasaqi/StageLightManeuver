using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;


namespace StageLightManeuver
{
    [CustomEditor(typeof(StageLightProfile), true)]
    // Custom Editor stage light profile
    public class StageLightProfileEditor : Editor
    {
        public StageLightProfile stageLightProfile;
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            stageLightProfile = target as StageLightProfile;
            var stageLightProperties = stageLightProfile.stageLightProperties;
            var serializedProperty = serializedObject.FindProperty("stageLightProperties");
            // stageLightProperties.RemoveAll(x => x == null);
            // stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));

            EditorGUI.BeginChangeCheck();
            EditorUtility.SetDirty(stageLightProfile);
            var drawer = new StageLightPropertiesDrawer();
            drawer.OnGUI(EditorGUILayout.GetControlRect(), serializedProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                serializedProperty.serializedObject.ApplyModifiedProperties();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Repaint();
                ReloadPreviewInstances();
            }            
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }

        // private void DrawAddPropertyButton(StageLightProfile stageLightProfile)
        // {
        //     EditorGUI.BeginChangeCheck();
        //     var selectList = new List<string>();

        //     SlmEditorUtility.SlmPropertyTypes.ForEach(t =>
        //     {
        //         if (t != typeof(RollProperty)) selectList.Add(t.Name);
        //     });

        //     selectList.Insert(0, "Add Property");
        //     foreach (var property in stageLightProfile.stageLightProperties)
        //     {
        //         if (property == null) continue;
        //         if (selectList.Find(x => x == property.GetType().Name) != null)
        //         {
        //             selectList.Remove(property.GetType().Name);
        //         }
        //     }
        //     EditorGUI.BeginDisabledGroup(selectList.Count <= 1);
        //     var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.MinWidth(200));
        //     EditorGUI.EndDisabledGroup();
        //     if (EditorGUI.EndChangeCheck())
        //     {
        //         EditorUtility.SetDirty(stageLightProfile);
        //         var type = SlmEditorUtility.GetTypeByClassName(selectList[select]);
        //         var property = Activator.CreateInstance(type) as SlmProperty;
        //         if (property.GetType() == typeof(ManualLightArrayProperty))
        //         {
        //             var manualLightArrayProperty = property as ManualLightArrayProperty;
        //             var lightProperty = stageLightProfile.TryGet<LightProperty>();
        //             var lightIntensityProperty = stageLightProfile.TryGet<LightIntensityProperty>();
        //             if (lightProperty != null)
        //             {
        //                 manualLightArrayProperty.initialValue.angle = lightProperty.spotAngle.value.constant;
        //                 manualLightArrayProperty.initialValue.innerAngle = lightProperty.innerSpotAngle.value.constant;
        //                 manualLightArrayProperty.initialValue.range = lightProperty.range.value.constant;
        //             }
        //             if (lightIntensityProperty != null)
        //             {
        //                 manualLightArrayProperty.initialValue.intensity = lightIntensityProperty.lightToggleIntensity.value.constant;
        //             }
        //         }
        //         stageLightProfile.stageLightProperties.Add(property);

        //         AssetDatabase.SaveAssets();
        //         serializedObject.ApplyModifiedProperties();
        //         AssetDatabase.Refresh();
        //         Repaint();
        //         ReloadPreviewInstances();
        //     }


        // }




    }

}