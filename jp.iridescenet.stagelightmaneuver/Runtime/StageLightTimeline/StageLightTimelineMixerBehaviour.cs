using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using StageLightManeuver;

public class StageLightTimelineMixerBehaviour : PlayableBehaviour
{

    public List<TimelineClip> clips;

    private bool firstFrameHappend = false;
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        StageLightSupervisor trackBinding = playerData as StageLightSupervisor;

        if (!trackBinding)
            return;

        if (firstFrameHappend)
        {
            trackBinding.Init();
            firstFrameHappend = true;
        }

        int inputCount = playable.GetInputCount ();
        var time = playable.GetTime();
        for (int i = 0; i < clips.Count; i++)
        {
            var clip = clips[i];
            var stageLightTimelineClip = clip.asset as StageLightTimelineClip;
            if(stageLightTimelineClip == null) continue;
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<StageLightTimelineBehaviour> inputPlayable = (ScriptPlayable<StageLightTimelineBehaviour>)playable.GetInput(i);
            // StageLightTimelineBehaviour input = inputPlayable.GetBehaviour ();
            var timeProperty = stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<TimeProperty>();
            if (timeProperty != null)
            {
                timeProperty.clipProperty.clipStartTime = (float)clip.start;
                timeProperty.clipProperty.clipEndTime = (float)clip.end;    
            }
            //
            var manualPanTiltProperty = stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.TryGet<ManualPanTiltProperty>();
            //
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
                
                if(manualPanTiltArray.Count > trackBinding.AllStageLights.Count)
                {
                    while (manualPanTiltArray.Count > trackBinding.AllStageLights.Count)
                    {
                        manualPanTiltArray.RemoveAt(manualPanTiltArray.Count - 1);
                    }
                }

                for (int j = 0; j < trackBinding.AllStageLights.Count; j++)
                {
                    manualPanTiltArray[j].name = trackBinding.AllStageLights[j].name;
                }

            }
            if (inputWeight > 0)
            {
                stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData.weight = inputWeight;
                trackBinding.AddQue(stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData);
            }
        }
        
        trackBinding.EvaluateQue((float)time);
    }
}
