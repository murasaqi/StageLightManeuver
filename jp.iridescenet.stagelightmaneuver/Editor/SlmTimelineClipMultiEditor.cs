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
        }

        public VisualElement profileInputWrapper;
        
        public StageLightProfile stageLightProfile;
        
        public List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
        public TextField selectedClipsField;
        private StringBuilder stringBuilder = new StringBuilder();
        public ObjectField stageLightProfileField;
        public Dictionary<Toggle,SlmProperty> toggleProperties = new Dictionary<Toggle, SlmProperty>();

        private VisualElement propertyList;
        
        [SerializeReference]private StageLightProfile stageLightProfileCopy;
        
        public void CreateGUI()
        {
            stageLightProfileCopy = CreateInstance<StageLightProfile>();
            
            var root = rootVisualElement;
           
            var wrapper =  new VisualElement();
            root.Add(wrapper);
            
            Selection.selectionChanged += () =>
            {
                SelectClips();
                InitAndProperties();
            };
            profileInputWrapper = new VisualElement();
            profileInputWrapper.style.flexDirection = FlexDirection.Row;
            wrapper.Add(profileInputWrapper);
            
            
            propertyList = new VisualElement();
            selectedClipsField = new TextField("Select clips");
            selectedClipsField.isReadOnly = true;
            selectedClipsField.multiline = true;
            selectedClipsField.style.maxHeight = 100;
            // selectedClipsField.style.textOverflow = TextOverflow.
            stageLightProfileField = new ObjectField("StageLightProfile");
            stageLightProfileField.objectType = typeof(StageLightProfile);
            stageLightProfileField.RegisterValueChangedCallback(evt =>
            {
                stageLightProfile = evt.newValue == null ? null : evt.newValue as StageLightProfile;
                InitStageLightPropertyList();
            });

            profileInputWrapper.Add(stageLightProfileField);

            profileInputWrapper.Add(CreateProfilePopup());
            
            var button = new Button();
            button.text = "Apply Profile";
            button.clickable.clicked += () =>
            {
                foreach (var selectedClip in selectedClips)
                {
                    if (stageLightProfileField.value != null)
                    {
                        selectedClip.referenceStageLightProfile = stageLightProfile;
                    }
                    else
                    {
                        selectedClip.referenceStageLightProfile = null;
                        selectedClip.syncReferenceProfile = false;
                    }
                    selectedClip.forceTimelineClipUpdate = true;
                    selectedClip.InitSyncData();
                }
            };
            
            wrapper.Add(foldout);
            
            wrapper.Add(button);
            
            var applyPropertyButton = new Button();
            applyPropertyButton.text = "Apply Property";
            
            applyPropertyButton.clickable.clicked += () =>
            {
                foreach (var selectedClip in selectedClips)
                {
                    selectedClip.syncReferenceProfile = false;
                    selectedClip.referenceStageLightProfile = null;
                    
                    foreach (var property in toggleProperties)
                    {
                        if(property.Key.value == false) continue;
                        var stageLightProperties = selectedClip.behaviour.stageLightQueData.stageLightProperties;
                        
                        AddOrOverwriteProperty(stageLightProperties, property.Value);
                        
                        // stageLightProperties.Sort( (a,b) => a.propertyName.CompareTo(b.propertyName));
                    }
                    
                }
            };
            wrapper.Add(propertyList);
            wrapper.Add(applyPropertyButton);
            // var stageLightProfileCopyField = new PropertyField();
            // stageLightProfileCopyField.BindProperty(new SerializedObject(stageLightProfileCopy).FindProperty("stageLightProperties"));
            
            // stageLightProfileCopyField.RegisterValueChangeCallback((v) =>
            // {
            //     Debug.Log(v.changedProperty);
            // });
            // var foldout = new Foldout();
            // foldout.text = "Multi Property Edit";
            // foldout.Add(stageLightProfileCopyField);
            // foldout.style.paddingLeft = 10;
            // foldout.value = false;
            //
            // var applyMultiPropertyButton = new Button();
            // applyPropertyButton.clicked += () =>
            // {
            //     foreach (var selectedClip in selectedClips)
            //     {
            //         selectedClip.syncReferenceProfile = false;
            //         selectedClip.referenceStageLightProfile = null;
            //         
            //         foreach (var property in stageLightProfileCopy.stageLightProperties)
            //         {
            //             var stageLightProperties = selectedClip.behaviour.stageLightQueData.stageLightProperties;
            //             
            //             AddOrOverwriteProperty(stageLightProperties, property);
            //         }
            //     }
            //
            // };
            // applyMultiPropertyButton.text = "Apply Multi Property";
            // foldout.Add(applyMultiPropertyButton);
            
            // wrapper.Add(foldout);

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


        public void InitStageLightPropertyList()
        {
            toggleProperties.Clear();
            propertyList.Clear();
            if(stageLightProfile == null) return;
            foreach (var property in stageLightProfile.stageLightProperties)
            {
                var toggle = new Toggle(property.propertyName) { value = false };
                propertyList.Add(toggle);
                toggleProperties.Add(toggle,property);
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
                    if (propertyTypes.Contains(property.GetType())) continue;
                    propertyTypes.Add(property.GetType());
                }
            }
            
            foreach (var propertyType in propertyTypes)
            {
                var property = System.Activator.CreateInstance(propertyType) as SlmProperty;
                stageLightProfileCopy.stageLightProperties.Add(property);
            }

           
        }


        public void OverwriteProperty()
        {
            
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
            selectedClipsField.value = "";
            stringBuilder.Clear();
            
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
                        stringBuilder.AppendLine(timelineClip.displayName);
                        var asset = timelineClip.asset as StageLightTimelineClip;
                        
                        selectedClips.Add(asset);

                    }
                }
                
            }
            
            selectedClipsField.value = stringBuilder.ToString();
         
        }
    }
     
    
}
