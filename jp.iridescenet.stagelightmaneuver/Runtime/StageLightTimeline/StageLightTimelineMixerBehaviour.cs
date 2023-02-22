using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StageLightManeuver
{
    public class StageLightTimelineMixerBehaviour : PlayableBehaviour
    {

        public List<TimelineClip> clips;

        public StageLightTimelineTrack stageLightTimelineTrack;
        private bool firstFrameHapend = false;

        public StageLightSupervisor trackBinding;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as StageLightSupervisor;

            if (!trackBinding)
                return;

            if (firstFrameHapend)
            {
                trackBinding.Init();
                firstFrameHapend = true;
            }


            var hasAnyClipPlaying = false;
            var time = playable.GetTime();
            for (int i = 0; i < clips.Count; i++)
            {
                var clip = clips[i];
                var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
                if (stageLightTimelineClip == null) continue;
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<StageLightTimelineBehaviour> inputPlayable =
                    (ScriptPlayable<StageLightTimelineBehaviour>)playable.GetInput(i);
                // StageLightTimelineBehaviour input = inputPlayable.GetBehaviour ();
                var timeProperty = stageLightTimelineClip.behaviour.stageLightQueData.TryGet<TimeProperty>();
                if (timeProperty != null)
                {
                    timeProperty.clipProperty.clipStartTime = (float)clip.start;
                    timeProperty.clipProperty.clipEndTime = (float)clip.end;
                }

                var manualPanTiltProperty =
                    stageLightTimelineClip.behaviour.stageLightQueData.TryGet<ManualPanTiltProperty>();
                FetchManualPanTiltArraySize(manualPanTiltProperty);

                var manualLightArray =
                    stageLightTimelineClip.behaviour.stageLightQueData.TryGet<ManualLightArrayProperty>();
                FetchManualLightArraySize(manualLightArray);
                
                var manualColorArray =
                    stageLightTimelineClip.behaviour.stageLightQueData.TryGet<ManualColorArrayProperty>();
                FetchManualColorArraySize(manualColorArray);
                
                if (inputWeight > 0)
                {
                    stageLightTimelineClip.behaviour.stageLightQueData.weight = inputWeight;
                    trackBinding.AddQue(stageLightTimelineClip.behaviour.stageLightQueData);
                    hasAnyClipPlaying = true;
                    // Debug.Log($"{clip.displayName},{inputWeight}");
                }
            }

            if (stageLightTimelineTrack)
            {
                if (!hasAnyClipPlaying)
                {
                    if (stageLightTimelineTrack.updateOnOutOfClip) trackBinding.EvaluateQue((float)time);
                    trackBinding.UpdateFixture();
                }
                else
                {
                    trackBinding.EvaluateQue((float)time);
                    trackBinding.UpdateFixture();
                }
            }

        }

        internal void FetchManualPanTiltArraySize(ManualPanTiltProperty manualPanTiltProperty)
        {

            if (manualPanTiltProperty != null)
            {
                var manualPanTiltArray = manualPanTiltProperty.positions.value;
                if (manualPanTiltArray.Count < trackBinding.AllStageLights.Count)
                {
                    while (manualPanTiltArray.Count < trackBinding.AllStageLights.Count)
                    {
                        manualPanTiltArray.Add(new PanTiltPrimitive());
                    }

                }

                if (manualPanTiltArray.Count > trackBinding.AllStageLights.Count)
                {
                    while (manualPanTiltArray.Count > trackBinding.AllStageLights.Count)
                    {
                        manualPanTiltArray.RemoveAt(manualPanTiltArray.Count - 1);
                    }
                }

                for (int j = 0; j < trackBinding.AllStageLights.Count; j++)
                {
                    // if not index is out of range
                    if (j < manualPanTiltArray.Count && j < trackBinding.AllStageLights.Count)
                    {
                        if (manualPanTiltArray[j] != null && trackBinding.AllStageLights[j] != null)
                        {
                            manualPanTiltArray[j].name = trackBinding.AllStageLights[j].name;
                        }

                    }

                }

            }
        }
        

        internal void FetchManualLightArraySize(ManualLightArrayProperty manualLightArray)
        {

            if (manualLightArray != null)
            {
                var lightPrimitiveValues = manualLightArray.lightValues.value;
                if (lightPrimitiveValues.Count < trackBinding.AllStageLights.Count)
                {
                    while (lightPrimitiveValues.Count < trackBinding.AllStageLights.Count)
                    {
                        lightPrimitiveValues.Add(new LightPrimitiveValue());
                    }

                }

                if (lightPrimitiveValues.Count > trackBinding.AllStageLights.Count)
                {
                    while (lightPrimitiveValues.Count > trackBinding.AllStageLights.Count)
                    {
                        lightPrimitiveValues.RemoveAt(lightPrimitiveValues.Count - 1);
                    }
                }

                for (int j = 0; j < trackBinding.AllStageLights.Count; j++)
                {
                    // if not index is out of range
                    if (j < lightPrimitiveValues.Count && j < trackBinding.AllStageLights.Count)
                    {
                        if (lightPrimitiveValues[j] != null && trackBinding.AllStageLights[j] != null)
                        {
                            lightPrimitiveValues[j].name = trackBinding.AllStageLights[j].name;
                        }

                    }

                }

            }
        }
        
        
        internal void FetchManualColorArraySize (ManualColorArrayProperty manualColorArray)
        {

            if (manualColorArray != null)
            {
                var colorPrimitiveValues = manualColorArray.colorValues.value;
                if (colorPrimitiveValues.Count < trackBinding.AllStageLights.Count)
                {
                    while (colorPrimitiveValues.Count < trackBinding.AllStageLights.Count)
                    {
                        colorPrimitiveValues.Add(new ColorPrimitiveValue());
                    }

                }

                if (colorPrimitiveValues.Count > trackBinding.AllStageLights.Count)
                {
                    while (colorPrimitiveValues.Count > trackBinding.AllStageLights.Count)
                    {
                        colorPrimitiveValues.RemoveAt(colorPrimitiveValues.Count - 1);
                    }
                }

                for (int j = 0; j < trackBinding.AllStageLights.Count; j++)
                {
                    // if not index is out of range
                    if (j < colorPrimitiveValues.Count && j < trackBinding.AllStageLights.Count)
                    {
                        if (colorPrimitiveValues[j] != null && trackBinding.AllStageLights[j] != null)
                        {
                            colorPrimitiveValues[j].name = trackBinding.AllStageLights[j].name;
                        }

                    }

                }

            }
        }
    }
}