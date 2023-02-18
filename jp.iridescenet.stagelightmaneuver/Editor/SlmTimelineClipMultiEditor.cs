using System.Collections.Generic;
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


        public StageLightProfile stageLightProfile;
        public List<StageLightTimelineClip> selectedClips = new List<StageLightTimelineClip>();
        public Label debugLabel;
        private StringBuilder stringBuilder = new StringBuilder();
        public void CreateGUI()
        {
            
            var root = rootVisualElement;
           
            var wrapper =  new VisualElement();
            root.Add(wrapper);
            wrapper.Add(new Label("hoge"));
            
            Selection.selectionChanged += () =>
            {
                SelectClips();
            };
            
            debugLabel = new Label();

            var objectField = new ObjectField();
            objectField.objectType = typeof(StageLightProfile);
            objectField.RegisterValueChangedCallback(evt =>
            {
                stageLightProfile = evt.newValue == null ? null : evt.newValue as StageLightProfile;
            });
            
            
            wrapper.Add(objectField);

            var button = new Button();
            button.text = "Apply";
            button.clickable.clicked += () =>
            {
                foreach (var selectedClip in selectedClips)
                {
                    if (objectField.value != null)
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
            
            wrapper.Add(button);
            wrapper.Add(debugLabel);
        }

        public void SelectClips()
        {
            var select = Selection.objects;

            selectedClips.Clear();
            debugLabel.text = "";
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
                
                debugLabel.text = stringBuilder.ToString();
            }
        }
    }
     
    
}
