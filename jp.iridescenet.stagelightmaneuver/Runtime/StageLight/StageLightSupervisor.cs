using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class StageLightSupervisor: MonoBehaviour
    {
        public List<StageLightBase> stageLights = new List<StageLightBase>();
        public List<StageLightSupervisor> stageLightSupervisors = new List<StageLightSupervisor>();
        [SerializeReference]private List<StageLightBase> allStageLights = new List<StageLightBase>();

        public List<StageLightBase> AllStageLights => allStageLights;

        [ContextMenu("Initialize")]
        public void Init()
        {
            allStageLights.Clear();
            allStageLights.AddRange(stageLights);
            foreach (var stageLightSupervisor in stageLightSupervisors)
            {
                allStageLights.AddRange(stageLightSupervisor.allStageLights);
            }
            
            
            
            var index = 0;
            foreach (var stageLight in allStageLights)
            {
                if (stageLight)
                {
                    stageLight.Index = index;
                    stageLight.Init();
                    index++;
                }
                
            }
        }

        
        [ContextMenu("Find Stage Lights in Children")]
        public void FindStageLightsInChildren()
        {
            stageLights.Clear();
            stageLights.AddRange(GetComponentsInChildren<StageLightBase>());
            Init();
        }
        
        private void OnValidate()
        {
            Init();
        }

        public void AddQue(StageLightQueueData stageLightQueData)
        {
            foreach (var stageLight in allStageLights)
            {
                if(stageLight != null)stageLight.AddQue(stageLightQueData);
            }
        }

        public void EvaluateQue(float time)
        {
            foreach (var stageLight in allStageLights)
            {
                 if(stageLight != null)stageLight.EvaluateQue(time);
            }
        }
        
        
        
        public List<Type> GetAllPropertyType()
        {
            var types = new List<Type>();
            foreach (var stageLight in AllStageLights)
            {

                if (stageLight.GetType() == typeof(StageLight))
                {
                    StageLight sl = (StageLight) stageLight;
                    types.AddRange(sl.StageLightFixtures.SelectMany(fixture => fixture.PropertyTypes));
                }
            }

            // remove same type from list
            types = types.Distinct().ToList();
            return types;
        }

        public void UpdateFixture()
        {
            foreach (var stageLightBase in allStageLights)
            {
                if(stageLightBase != null)stageLightBase.UpdateFixture();
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