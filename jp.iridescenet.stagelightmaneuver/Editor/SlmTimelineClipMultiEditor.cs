using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace StageLightManeuver
{
    public class SlmTimelineClipMultiEditor : EditorWindow
    {
   
        [MenuItem("Window/SlmTimelineClipMultiEditor")]
        public static void ShowExample()
        {
            var window = GetWindow<SlmTimelineClipMultiEditor>();
            window.titleContent = new GUIContent("SlmTimelineClipMultiEditor");
            
            stageLightProfileCopy = CreateInstance<StageLightProfile>();
            stageLightProfileCopy.stageLightProperties = new List<SlmProperty>();
           
        }
        
        [SerializeReference]private static StageLightProfile stageLightProfileCopy;

        public VisualElement profileInputWrapper;
        
        public StageLightProfile stageLightProfile;
        // public StageLightProfile multi
        public List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
        public List<SlmProperty> slmProperties = new List<SlmProperty>();
        public TextField selectedClipsField;
        private StringBuilder stringBuilder = new StringBuilder();
        public ObjectField stageLightProfileField;
        // public Dictionary<Toggle,SlmProperty> toggleProperties = new Dictionary<Toggle, SlmProperty>();

        // private VisualElement propertyList;
        private Vector2 slmPropertyScrollPosition = Vector2.zero;
        private Vector2 selectedClipScrollPosition = Vector2.zero;
        // public 
        
        public void OnGUI()
        {
            
            Selection.selectionChanged += () =>
            {
                selectedClips = SlmEditorUtility.SelectClips();
                SlmEditorUtility.InitAndProperties( stageLightProfileCopy,selectedClips);
                Repaint();
            };

           
            if (stageLightProfileCopy == null)
            {
                
                stageLightProfileCopy = CreateInstance<StageLightProfile>();
            }
            
            
            var ownSerializedObject = new SerializedObject(this);
            var selectedClipsProperty = ownSerializedObject.FindProperty("selectedClips");
            EditorGUILayout.PropertyField(selectedClipsProperty,true);
            // EditorGUILayout.EndScrollView();
            
          
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Found Properties",new GUIStyle()
            {
                richText = true,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = new Color(0.8f,0.8f,0.8f)
                }
            });
           
            slmPropertyScrollPosition = EditorGUILayout.BeginScrollView(slmPropertyScrollPosition);
  
            
            var serializedObject = new SerializedObject(stageLightProfileCopy);
            var stageLightPropertiesProperty = serializedObject.FindProperty("stageLightProperties");
            for (int i = 0; i < stageLightProfileCopy.stageLightProperties.Count; i++)
            {   
                var property = stageLightProfileCopy.stageLightProperties[i];
                if (property == null)
                {
                    continue;
                }
                var serializedProperty = stageLightPropertiesProperty.GetArrayElementAtIndex(i);
                // serializedProperty.isExpanded = true;
                StageLightProfileEditorUtil.DrawStageLightProperty(stageLightProfileCopy.stageLightProperties,serializedProperty ,false);

                GUILayout.Space(2);
                using (new EditorGUILayout.HorizontalScope())
                {

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("☑ Apply checked properties", GUILayout.Width(200)))
                    {
                        OverwriteProperties();
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(2);
            
            }

            EditorGUILayout.EndScrollView();
        }

        public void OverwriteProperties()
        {
            var properties = stageLightProfileCopy.stageLightProperties.FindAll(x => x.propertyOverride == true);

            foreach (var p in properties)
            {
                if(p.propertyOverride == false) continue;
                foreach (var selectedClip in selectedClips)
                {
                    Debug.Log(p.GetType());
                    if(selectedClip.stageLightProfile == null) continue;

                    foreach (var property in selectedClip.stageLightProfile.stageLightProperties)
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
       
        public void AddOrOverwriteProperty(List<SlmProperty> slmProperties, SlmProperty property)
        {
            var sameTypeProperty = slmProperties.Find(p => p.GetType() == property.GetType());
            var type = property.GetType();
            var copyProperty = Activator.CreateInstance(type, BindingFlags.CreateInstance, null,
                    new object[] { property }, null)
                as SlmProperty;
            if (sameTypeProperty != null)
            {
                
                slmProperties[slmProperties.IndexOf(sameTypeProperty)] = copyProperty;
            }
            else
            {
                slmProperties.Add(property);
            }
        }

        public void InitAndProperties()
        {
            stageLightProfileCopy.stageLightProperties.Clear();
            var propertyTypes = new List<System.Type>();
            
            foreach (var selectedClip in selectedClips)
            {
                foreach (var property in selectedClip.stageLightProfile.stageLightProperties)
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


        public PopupField<string> CreateProfilePopup()
        {
            var allProfilesInProject = SlmUtility.GetProfileInProject();
            var profileNames = new List<string>();
            var folderNamesProfileDict = new Dictionary<string, List<StageLightProfile>>();
            foreach (var profile in allProfilesInProject)
            {
                var path = AssetDatabase.GetAssetPath(profile);
                var parentDirectory = Path.GetDirectoryName(path).Replace("Assets/", "").Replace("Assets\\", "");
                parentDirectory = parentDirectory.Replace("\\", ">").Replace("/", ">");
                if (folderNamesProfileDict.ContainsKey(parentDirectory))
                {
                    folderNamesProfileDict[parentDirectory].Add(profile);
                }
                else
                {
                    folderNamesProfileDict.Add(parentDirectory, new List<StageLightProfile> {profile});
                }

            }

            foreach (var keyPair in folderNamesProfileDict)
            {
                foreach (var v in keyPair.Value)
                {
                    profileNames.Add($"{keyPair.Key}/{v.name}");
                }
            }
            
            var popup = new PopupField<string>(profileNames, 0);
            popup.RegisterValueChangedCallback(evt =>
            {
                var index = profileNames.IndexOf(evt.newValue);
                stageLightProfileField.value = allProfilesInProject[index];
            });
            return popup;
        }
        
    }
     
    
}
