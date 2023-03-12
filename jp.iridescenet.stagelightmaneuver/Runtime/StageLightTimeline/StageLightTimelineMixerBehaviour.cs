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
                var timeProperty = stageLightTimelineClip.stageLightProfile.TryGet<ClockProperty>();
                if (timeProperty != null)
                {
                    timeProperty.clipProperty.clipStartTime = (float)clip.start;
                    timeProperty.clipProperty.clipEndTime = (float)clip.end;
                }

                foreach (var stageLightProperty in stageLightTimelineClip.stageLightProfile.stageLightProperties)
                {
                    if(stageLightProperty == null) continue;
                    if (stageLightProperty.GetType().IsSubclassOf (typeof(SlmAdditionalArrayProperty)))
                    {
                        var additionalArrayProperty = stageLightProperty as SlmAdditionalArrayProperty;
                        additionalArrayProperty?.ResyncArraySize(trackBinding);
                    }
                }
                
                if (inputWeight > 0)
                {
                    stageLightTimelineClip.stageLightProfile.weight = inputWeight;
                    trackBinding.AddQue(stageLightTimelineClip.stageLightProfile);
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

       
        

        
        
    }
}