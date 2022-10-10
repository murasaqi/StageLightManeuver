using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public abstract class StageLight: MonoBehaviour,IStageLight
    {
        
        [SerializeField]private int index = 0;
        public int Index { get => index; set => index = value; }
        [SerializeField]private List<StageLight> stageLightChild = new List<StageLight>();

        public List<StageLight> StageLightChild
        {
            get => stageLightChild;
            set=> stageLightChild = value;
        }
        

        private void Update()
        {
            
        }

        public virtual void AddQue(StageLightQueData stageLightQueData)
        {
            
            foreach (var stageLight in StageLightChild)
            {
                stageLight.AddQue(stageLightQueData);
            }
        }

        public virtual void EvaluateQue(float time)
        {
            var i = 0;
            foreach (var stageLight in StageLightChild)
            {
                // Debug.Log(stageLight.name);
                stageLight.Index = i;
                stageLight.EvaluateQue(time);
                i++;
            }
           
        }
        
        
        [ContextMenu("Get StageLights in Children")]
        public void AddStageLightInChild()
        {
            stageLightChild.Clear();
            stageLightChild = GetComponentsInChildren<StageLight>().ToList();

            if (stageLightChild == null || stageLightChild.Count == 0)
                return;
            for (int i = stageLightChild.Count ; i > 0; i--)
            {
                if (stageLightChild[i-1] == this)
                {
                    stageLightChild.RemoveAt(i-1);
                }
            }
            
        }
        
       

    }
}
