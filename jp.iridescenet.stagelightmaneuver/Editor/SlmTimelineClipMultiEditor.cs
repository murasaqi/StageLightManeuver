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
        public Label selectedClipsField;
        private StringBuilder stringBuilder = new StringBuilder();
        public ObjectField stageLightProfileField;
        public Dictionary<Toggle,SlmProperty> toggleProperties = new Dictionary<Toggle, SlmProperty>();

        private VisualElement propertyList;
        public void CreateGUI()
        {
            
            var root = rootVisualElement;
           
            var wrapper =  new VisualElement();
            root.Add(wrapper);
            
            Selection.selectionChanged += () =>
            {
                SelectClips();
            };
            profileInputWrapper = new VisualElement();
            profileInputWrapper.style.flexDirection = FlexDirection.Row;
            wrapper.Add(profileInputWrapper);
            
            
            propertyList = new VisualElement();
            selectedClipsField = new Label("Select clips");
            selectedClipsField.SetEnabled(false);
            stageLightProfileField = new ObjectField("StageLightProfile");
            stageLightProfileField.objectType = typeof(StageLightProfile);
            stageLightProfileField.RegisterValueChangedCallback(evt =>
            {
                stageLightProfile = evt.newValue == null ? null : evt.newValue as StageLightProfile;
                toggleProperties.Clear();
                propertyList.Clear();
                foreach (var property in stageLightProfile.stageLightProperties)
                {
                    var toggle = new Toggle(property.propertyName) { value = true };
                    propertyList.Add(toggle);
                    toggleProperties.Add(toggle,property);
                }
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
            
            wrapper.Add(selectedClipsField);
            
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
                        
                        // if same type in stageLightProperties, overwrite
                        var sameTypeProperty = stageLightProperties.FirstOrDefault(p => p.GetType() == property.Value.GetType());
                        if (sameTypeProperty != null)
                        {
                            stageLightProperties.Remove(sameTypeProperty);
                        } 
                        stageLightProperties.Add(property.Value);
                    }
                    selectedClip.forceTimelineClipUpdate = true;
                }
            };
            wrapper.Add(propertyList);
            wrapper.Add(applyPropertyButton);
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
            selectedClipsField.text = "";
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
            
            selectedClipsField.text = stringBuilder.ToString();
         
        }
    }
     
    
}
