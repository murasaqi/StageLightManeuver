using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLightSupervisor: MonoBehaviour
    {
        public List<StageLightBase> stageLights = new List<StageLightBase>();
        public List<StageLightSupervisor> stageLightSupervisors = new List<StageLightSupervisor>();
        // [Range(0f,1f)]public float fader = 0f;

        // public int a;
        // public int b;

        [SerializeReference]private List<StageLightBase> allStageLights = new List<StageLightBase>();

        public List<StageLightBase> AllStageLights => allStageLights;


        [ContextMenu("Initialize")]
        public void Init()
        {
            allStageLights.Clear();
            allStageLights.AddRange(stageLights);
            foreach (var stageLightSupervisor in stageLightSupervisors)
            {
                allStageLights.AddRange(stageLightSupervisor.stageLights);
            }
            
            var index = 0;
            foreach (var stageLight in allStageLights)
            {
                if (stageLight)
                {
                    stageLight.Index = index;
                    index++;
                }
                
            }
        }

        
        [ContextMenu("Find Stage Lights in Children")]
        public void FindStageLightsInChildren()
        {
            stageLights.Clear();
            stageLights.AddRange(GetComponentsInChildren<StageLightBase>());
        }
        
        private void OnValidate()
        {
            Init();
        }

        public void AddQue(StageLightQueData stageLightQueData)
        {
            foreach (var stageLight in allStageLights)
            {
                if(stageLight != null)stageLight.AddQue(stageLightQueData);
            }
        }

        public void EvaluateQue(float time)
        {
            foreach (var stageLight in stageLights)
            {
                 if(stageLight != null)stageLight.EvaluateQue(time);
            }
        }
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