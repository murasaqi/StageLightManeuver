using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace iridescent
{
    public class TimelineClipHelper : EditorWindow
    {
   
        [MenuItem("Window/TimelineClipHelper")]
        public static void ShowExample()
        {
            var window = GetWindow<TimelineClipHelper>();
            window.titleContent = new GUIContent("TimelineClipHelper");
        }


        public void CreateGUI()
        {
            
            var root = rootVisualElement;
           
            var wrapper =  new VisualElement();
            root.Add(wrapper);
            wrapper.Add(new Label("hoge"));
            var button = new Button();
            button.clickable.clicked += () =>
            {
                var select = Selection.objects;

                foreach (var s in select)
                {
                    if (s.GetType().ToString() == "UnityEditor.Timeline.EditorClip")
                    {
                        var clip = s.GetType().GetField("m_Clip", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(s);
                    
                        var timelineClip = clip as TimelineClip;
                        Debug.Log(timelineClip.displayName);     
                    }
                }
            };
            wrapper.Add(button);
        }
    }
     
    
}
