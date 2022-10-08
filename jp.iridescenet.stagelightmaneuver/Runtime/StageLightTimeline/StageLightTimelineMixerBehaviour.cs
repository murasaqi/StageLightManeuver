using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using StageLightManeuver;

public class StageLightTimelineMixerBehaviour : PlayableBehaviour
{

    public List<TimelineClip> clips;
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        StageLight trackBinding = playerData as StageLight;

        if (!trackBinding)
            return;

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
            
            if (inputWeight > 0)
            {
                trackBinding.AddQue(stageLightTimelineClip.stageLightTimelineBehaviour.stageLightQueData,inputWeight);
            }

        }
        
        trackBinding.UpdateFixture((float)time);
    }
}
