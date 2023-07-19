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
            stageLightProperties.RemoveAll(x => x == null);
            stageLightProperties.Sort((x, y) => x.propertyOrder.CompareTo(y.propertyOrder));

            for (int i = 0; i < stageLightProperties.Count; i++)
            {
                var slmProperty = stageLightProperties[i];
                if (slmProperty == null) continue;

                var serializedSlmProperty = serializedProperty.GetArrayElementAtIndex(i);
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
                    if(slmProperty.GetType() != typeof(ClockProperty))StageLightProfileEditorUtil.DrawRemoveButton(serializedObject, stageLightProperties, action);
                }
            }

            // DrawAddPropertyButton(stageLightTimelineClip);

            EditorGUI.EndDisabledGroup();
            // stageLightProfile = (StageLightProfile) target;
            // var stageLightPropertiesProperty = serializedObject.FindProperty("stageLightProperties");
            // var position = EditorGUILayout.GetControlRect();
            // Rect rect = EditorGUILayout.GetControlRect( );
            // EditorStyles.inspectorDefaultMargins.padding = new RectOffset(
            //     0,
            //     EditorStyles.inspectorDefaultMargins.padding.right,
            //     EditorStyles.inspectorDefaultMargins.padding.top,
            //     EditorStyles.inspectorDefaultMargins.padding.bottom
            // );
            // using (new EditorGUI.IndentLevelScope())
            // {
            //     for (int i = 0; i < stageLightProfile.stageLightProperties.Count; i++)
            //     {
            //         var property = stageLightProfile.stageLightProperties[i];
            //         if (property == null)
            //         {
            //             continue;
            //         } 
            //         StageLightProfileEditorUtil.DrawStageLightProperty(stageLightProfile.stageLightProperties, stageLightPropertiesProperty.GetArrayElementAtIndex(i),true);
            //     }
            // }
            // GUILayout.Space(2);
            // rect = EditorGUILayout.GetControlRect( );
            // EditorGUI.DrawRect(new Rect(rect.x,rect.y,rect.width,1), Color.black );
            // DrawAddPropertyButton(stageLightProfile);
            //
            //       


        }
        void OnInspectorUpdate()
        {
            Repaint();
        }

        private void DrawAddPropertyButton(StageLightProfile stageLightProfile)
        {
            EditorGUI.BeginChangeCheck();
            var selectList = new List<string>();

            SlmEditorUtility.SlmPropertyTypes.ForEach(t =>
            {
                if (t != typeof(RollProperty)) selectList.Add(t.Name);
            });

            selectList.Insert(0, "Add Property");
            foreach (var property in stageLightProfile.stageLightProperties)
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
                EditorUtility.SetDirty(stageLightProfile);
                var type = SlmEditorUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;
                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightProfile.TryGet<LightProperty>();
                    var lightIntensityProperty = stageLightProfile.TryGet<LightIntensityProperty>();
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
                stageLightProfile.stageLightProperties.Add(property);

                AssetDatabase.SaveAssets();
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.Refresh();
                Repaint();
                ReloadPreviewInstances();
            }


        }




    }

}