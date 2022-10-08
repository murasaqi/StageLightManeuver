using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLightSupervisor: MonoBehaviour
    {
        public List<StageLight> stageLights = new List<StageLight>();
        public List<StageLightProfile> stageLightSettings = new List<StageLightProfile>();
        [Range(0f,1f)]public float fader = 0f;

        public int a;
        public int b;
        
        void Update()
        {
            // if (a < stageLightSettings.Count)
            // {
            //     var stageLightSetting = stageLightSettings[a];
            //     foreach (var stageLight in stageLights)
            //     {
            //         stageLight.AddQue(stageLightSetting,fader);
            //     }
            // }
            //
            // if (b < stageLightSettings.Count)
            // {
            //     var stageLightSetting =stageLightSettings[b];
            //     foreach (var stageLight in stageLights)
            //     {
            //         stageLight.AddQue(stageLightSetting, 1f-fader);
            //     }
            // }
        }
    }
    
    
    
}