using System;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class RotationFixture:StageLightFixtureBase
    {
        public Transform target;
        public Vector3 rotationAxis = Vector3.up;
        public float rotationScalar = 0f;


        private void Start()
        {
            Init();
        }
        
        public override void Init()
        {
            base.Init();
               
        }

        public override void EvaluateQue(float time)
        {

            rotationAxis = Vector3.zero;
            rotationScalar = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var rotationProperty = queueData.TryGet<RotationProperty>() as RotationProperty;

                if (rotationProperty == null || stageLightBaseProperties == null)
                    return;

                var t = GetNormalizedTime(time, queueData, typeof(RotationProperty));

                rotationAxis += rotationProperty.rotationAxis.value * queueData.weight;
                rotationScalar += rotationProperty.rotationScalar.value * queueData.weight;
                
            }

        }

        public override void UpdateFixture()
        {
            
            if(target) target.eulerAngles = rotationAxis * rotationScalar;
        }
    }
    
    
}