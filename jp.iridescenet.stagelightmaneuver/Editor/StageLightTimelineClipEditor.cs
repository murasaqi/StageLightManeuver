using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver.StageLightTimeline.Editor
{
    [CustomTimelineEditor(typeof(StageLightTimelineClip))]
    public class StageLightTimelineClipEditor:ClipEditor
    {
         [InitializeOnLoad]
        class EditorInitialize
        {
            static EditorInitialize()
            {
                backgroundTexture = new Texture2D(1, 1);
                syncIconTexture = Resources.Load<Texture2D>("VLVLBMaterials/icon_sync");
                backgroundTexture.SetPixel(0, 0, Color.white);
                backgroundTexture.Apply();
           
            } 
            static PlayableDirector GetMasterDirector() { return TimelineEditor.masterDirector; }
        }
        Dictionary<StageLightTimelineClip, Texture2D> _gradientTextures = new Dictionary<StageLightTimelineClip, Texture2D>();
        // Dictionary<StageLightTimelineClip, Texture2D> _beatTextures = new Dictionary<StageLightTimelineClip, Texture2D>();

        private Dictionary<StageLightTimelineClip, List<float>>
            _beatPoint = new Dictionary<StageLightTimelineClip, List<float>>();
        private static Texture2D backgroundTexture;
        private static Texture2D syncIconTexture;
     
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            return new ClipDrawOptions
            {
                errorText = GetErrorText(clip),
                highlightColor = GetDefaultHighlightColor(clip),
                icons = Enumerable.Empty<Texture2D>(),
                tooltip = "Tooltip"
            };
        }
        
        public override void OnClipChanged(TimelineClip clip)
        {
        
            var stageLightTimelineClip = (StageLightTimelineClip)clip.asset;
            if (stageLightTimelineClip == null)
                return;
            GetGradientTexture(clip, true);
            if (stageLightTimelineClip.referenceStageLightProfile != null)
                clip.displayName = stageLightTimelineClip.referenceStageLightProfile.name;
            

        }
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);
       
        }


        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);


            if(syncIconTexture == null)syncIconTexture = Resources.Load<Texture2D>("VLVLBMaterials/icon_sync");
            var stageLightTimelineClip = (StageLightTimelineClip) clip.asset;
            var update = stageLightTimelineClip.forceTimelineClipUpdate;
            if (stageLightTimelineClip.referenceStageLightProfile != null)
            {
                if (stageLightTimelineClip.referenceStageLightProfile.isUpdateGuiFlag) update = true;
                stageLightTimelineClip.referenceStageLightProfile.isUpdateGuiFlag = false;
            }
            var tex = GetGradientTexture(clip, update);
            if(stageLightTimelineClip.track == null) return;
            var colorHeight = region.position.height * stageLightTimelineClip.track.colorLineHeight;
            var beatHeight = 2f;

            UpdateBeatPoint(clip);
            if (stageLightTimelineClip.track.drawBeat)
            {
                if (_beatPoint.ContainsKey(stageLightTimelineClip))
                {
                    foreach (var p in _beatPoint[stageLightTimelineClip])
                    {
                        var width = region.position.size.x;
                        EditorGUI.DrawRect(new Rect(width* p, 0, 1, region.position.height),
                            stageLightTimelineClip.track.beatLineColor);
                    }     

                   
                }

                var size = 10;
                for (float i = 0; i < size; i++)
                {

                    // var with = region.position.width;
                    // EditorGUI.DrawRect(new Rect(region.position.x + with*, 0, 1, region.position.height),
                    //     stageLightTimelineClip.track.beatLineColor);
                }    
               
            }

            if (tex)
                GUI.DrawTexture(
                    new Rect(region.position.x, region.position.height - colorHeight, region.position.width,
                        colorHeight), tex);


            var iconSize = 12;
            var margin = 4;
            if (stageLightTimelineClip.syncReferenceProfile)
                GUI.DrawTexture(new Rect(region.position.width - iconSize - margin, margin, iconSize, iconSize),
                    syncIconTexture, ScaleMode.ScaleAndCrop,
                    true,
                    0,
                    new Color(1, 1, 1, 0.5f), 0, 0);
            
            stageLightTimelineClip.forceTimelineClipUpdate = false;
        }

        private float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType)
        {
            
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset *0;
            var offsetTime = time + offset;
            var result = 0f;
            var t = (float)offsetTime % duration;
            var normalisedTime = t / duration;
            
            if (loopType == LoopType.Loop)
            {
                result = normalisedTime;     
            }else if (loopType == LoopType.PingPong)
            {
                result = Mathf.PingPong(offsetTime / duration, 1f);
            }
            else if(loopType == LoopType.Fixed)
            {
                result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, time);
            }
           
            return result;
        }



        public void UpdateBeatPoint(TimelineClip clip,float step = 0.1f)
        {
            var customClip = clip.asset as StageLightTimelineClip;
            var beatPointList = new List<float>();
            
            
            
            var timeProperty = customClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<TimeProperty>();
            
            if (timeProperty != null)
            {
                var preBeat = -1f;
                for (float i =(float) clip.start; i < (float)clip.end; i+=step)
                {
                    var bpm = timeProperty.bpm.value;
                    var bpmOffset =timeProperty.bpmOffset.value;
                    var bpmScale = timeProperty.bpmScale.value;
                    var loopType = timeProperty.loopType.value;
                    float offsetTime = (60 / bpmScale) * bpmOffset;
                    var t =GetNormalizedTime(i,bpm,bpmOffset,bpmScale,timeProperty.clipProperty,loopType);
                    
                    
                    if (preBeat> t)
                    {
                        beatPointList.Add(t);
                    }
                    
                    preBeat = t;
                }
            }
            
            if (_beatPoint.ContainsKey(customClip))
            {
                _beatPoint[customClip] = beatPointList;
            }
            else
            {
                _beatPoint.Add(customClip, beatPointList);    
            }



           
                 
        }
        
        Texture2D GetGradientTexture(TimelineClip clip, bool update = false)
        {
            Texture2D tex = Texture2D.whiteTexture;

            var customClip = clip.asset as StageLightTimelineClip;
            
            if (!customClip) return tex;

            if(customClip.stageLightTimelineBehaviour.stageLightQueData == null) return tex;
            
            var lightProperty = customClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<LightProperty>();
            if(lightProperty == null) return tex;
            if(lightProperty.lightToggleColor == null) return tex;
            var gradient = lightProperty.lightToggleColor.value;
            var intensity = lightProperty.lightToggleIntensity.value;
            if (update) 
            {
                _gradientTextures.Remove(customClip);
            }
            else
            {
                _gradientTextures.TryGetValue(customClip, out tex);
                if (tex) return tex;
            }
            tex = new Texture2D(64, 1);
            var b = (float)(clip.blendInDuration / clip.duration);
          
            for (int i = 0; i < tex.width; ++i)
            {
                var currentTime  =(float)clip.duration* (float)i / tex.width;
            
                // var lightProfile = lightProperty;
                
                var timeProperty = customClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<TimeProperty>();
                if (timeProperty != null)
                {
                    var bpm = timeProperty.bpm.value;
                    var bpmOffset = lightProperty.bpmOverrideData.value.bpmOverride
                        ? lightProperty.bpmOverrideData.value.bpmOffset
                        : timeProperty.bpmOffset.value;
                    var bpmScale = lightProperty.bpmOverrideData.value.bpmOverride
                        ? lightProperty.bpmOverrideData.value.bpmScale
                        : timeProperty.bpmScale.value;
                    var loopType = lightProperty.bpmOverrideData.value.bpmOverride
                        ? lightProperty.bpmOverrideData.value.loopType
                        : timeProperty.loopType.value;
                   
                    var t = GetNormalizedTime(currentTime,bpm, bpmScale, bpmScale,timeProperty.clipProperty, loopType);
                    
                    var color = gradient.Evaluate(t);
                    if (b > 0f) color.a = Mathf.Min(t / b, 1f);
                    var intensityValue = Mathf.Clamp(intensity.Evaluate(t),0,1);
                    color = new Color(color.r,
                        color.g,
                        color.b,
                        color.a*intensityValue);
                    tex.SetPixel(i, 0, color);     
                    
                }
            }

            
            
            tex.Apply();
            if (_gradientTextures.ContainsKey(customClip))
            {
                _gradientTextures[customClip] = tex;
            }
            else
            {
                _gradientTextures.Add(customClip, tex);    
            }

           
            return tex;
        }

        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            base.GetSubTimelines(clip, director, subTimelines);
        }
    }
    
    
}