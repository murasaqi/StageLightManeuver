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
                SelectClips();
                InitAndProperties();
                Repaint();
            };

           
            if (stageLightProfileCopy == null)
            {
                
                stageLightProfileCopy = CreateInstance<StageLightProfile>();
            }
            
            // EditorGUILayout.LabelField("Selected Clips");

            
            
            // selectedClipScrollPosition = EditorGUILayout.BeginScrollView(selectedClipScrollPosition,GUILayout.Height(100));
            // var currentPosition = EditorGUILayout.GetControlRect();
            // EditorGUI.DrawRect(new Rect(currentPosition.x,currentPosition.y,currentPosition.width,120), new Color(0.1f,0.1f,0.1f));
            // EditorGUI.DrawRect(new Rect(currentPosition.x+1,currentPosition.y+1,currentPosition.width-2,120-2f), new Color(0.2f,0.2f,0.2f));
            // foreach (var selectedClip in selectedClips)
            // {
            //    
            //     // EditorGUI.BeginDisabledGroup(true);
            //     EditorGUILayout.ObjectField(selectedClip.clipDisplayName,selectedClip,typeof(StageLightTimelineClip),false);
            //     // EditorGUI.EndDisabledGroup();
            // }
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
                StageLightProfileEditorUtil.DrawStageLightProperty(serializedObject,serializedProperty ,false);

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
        // public void CreateGUI()
        // {
        //     stageLightProfileCopy = CreateInstance<StageLightProfile>();
        //     stageLightProfileCopy.stageLightProperties = new List<SlmProperty>();
        //     var root = rootVisualElement;
        //    
        //     var wrapper =  new VisualElement();
        //     root.Add(wrapper);
        //     
        //     Selection.selectionChanged += () =>
        //     {
        //         SelectClips();
        //         InitAndProperties();
        //     };
        //     profileInputWrapper = new VisualElement();
        //     profileInputWrapper.style.flexDirection = FlexDirection.Row;
        //     wrapper.Add(profileInputWrapper);
        //     
        //     
        //     propertyList = new VisualElement();
        //     selectedClipsField = new TextField("Select clips");
        //     selectedClipsField.isReadOnly = true;
        //     selectedClipsField.multiline = true;
        //     selectedClipsField.style.maxHeight = 100;
        //     // selectedClipsField.style.textOverflow = TextOverflow.
        //     stageLightProfileField = new ObjectField("StageLightProfile");
        //     stageLightProfileField.objectType = typeof(StageLightProfile);
        //     stageLightProfileField.RegisterValueChangedCallback(evt =>
        //     {
        //         stageLightProfile = evt.newValue == null ? null : evt.newValue as StageLightProfile;
        //         InitStageLightPropertyList();
        //     });
        //
        //     profileInputWrapper.Add(stageLightProfileField);
        //
        //     profileInputWrapper.Add(CreateProfilePopup());
        //     wrapper.Add(selectedClipsField);
        //     var button = new Button();
        //     button.text = "Apply Profile";
        //     button.clickable.clicked += () =>
        //     {
        //         foreach (var selectedClip in selectedClips)
        //         {
        //             if (stageLightProfileField.value != null)
        //             {
        //                 selectedClip.referenceStageLightProfile = stageLightProfile;
        //             }
        //             else
        //             {
        //                 selectedClip.referenceStageLightProfile = null;
        //                 selectedClip.syncReferenceProfile = false;
        //             }
        //             selectedClip.forceTimelineClipUpdate = true;
        //             selectedClip.InitSyncData();
        //         }
        //     };
        //     
        //     
        //     wrapper.Add(button);
        //     
        //     var applyPropertyButton = new Button();
        //     applyPropertyButton.text = "Apply Property";
        //     
        //     applyPropertyButton.clickable.clicked += () =>
        //     {
        //         foreach (var selectedClip in selectedClips)
        //         {
        //             selectedClip.syncReferenceProfile = false;
        //             selectedClip.referenceStageLightProfile = null;
        //             
        //             foreach (var property in toggleProperties)
        //             {
        //                 if(property.Key.value == false) continue;
        //                 var stageLightProperties = selectedClip.behaviour.stageLightQueData.stageLightProperties;
        //                 
        //                 AddOrOverwriteProperty(stageLightProperties, property.Value);
        //                 
        //                 // stageLightProperties.Sort( (a,b) => a.propertyName.CompareTo(b.propertyName));
        //             }
        //             
        //         }
        //     };
        //     wrapper.Add(propertyList);
        //     wrapper.Add(applyPropertyButton);
        //     
        //     
        //     
        //   
        //
        // }
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
        public void SelectClips()
        {
            var select = Selection.objects.ToList();
            selectedClips.Clear();
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
            
            // selectedClipsField.value = stringBuilder.ToString();
         
        }
    }
     
    
}
