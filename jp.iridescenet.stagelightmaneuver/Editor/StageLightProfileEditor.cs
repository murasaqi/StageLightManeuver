using System;
using System.Collections;
using System.Collections.Generic;
using StageLightManeuver;
using StageLightManeuver.StageLightTimeline.Editor;
using UnityEditor;
using UnityEngine;
using Action = Unity.Plastic.Newtonsoft.Json.Serialization.Action;

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
            var position = EditorGUILayout.GetControlRect();
            Rect rect = EditorGUILayout.GetControlRect( );
            EditorStyles.inspectorDefaultMargins.padding = new RectOffset(
                0,
                EditorStyles.inspectorDefaultMargins.padding.right,
                EditorStyles.inspectorDefaultMargins.padding.top,
                EditorStyles.inspectorDefaultMargins.padding.bottom
            );
            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < stageLightProfile.stageLightProperties.Count; i++)
                {
                    var property = stageLightProfile.stageLightProperties[i];
                    if (property == null)
                    {
                        continue;
                    } 
                    StageLightProfileEditorUtil.DrawStageLightProperty(serializedObject, stageLightPropertiesProperty.GetArrayElementAtIndex(i));
                }
            }
            GUILayout.Space(2);
            rect = EditorGUILayout.GetControlRect( );
            EditorGUI.DrawRect(new Rect(rect.x,rect.y,rect.width,1), Color.black );
            DrawAddPropertyButton(stageLightProfile);
            
                  
            

        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        
        private void DrawAddPropertyButton(StageLightProfile stageLightProfile)
        {
            EditorGUI.BeginChangeCheck();
             var selectList = new List<string>();
            
            SlmUtility.SlmAdditionalTypes.ForEach(t =>
            {
                if(t != typeof(RollProperty))selectList.Add(t.Name);
            });
            
           selectList.Insert(0,"Add Property");
            foreach (var property in stageLightProfile.stageLightProperties)
            {
               if(property == null) continue;
                if (selectList.Find(x => x== property.GetType().Name) != null)
                {
                    selectList.Remove(property.GetType().Name);
                }
            }
            EditorGUI.BeginDisabledGroup(selectList.Count  <= 1);
            var select = EditorGUILayout.Popup(0, selectList.ToArray(), GUILayout.MinWidth(200));
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(stageLightProfile);   
                var type = SlmUtility.GetTypeByClassName(selectList[select]);
                var property = Activator.CreateInstance(type) as SlmProperty;
                if (property.GetType() == typeof(ManualLightArrayProperty))
                {
                    var manualLightArrayProperty = property as ManualLightArrayProperty;
                    var lightProperty = stageLightProfile.TryGet<LightProperty>();
                    var lightIntensityProperty = stageLightProfile.TryGet<LightIntensityProperty>();
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