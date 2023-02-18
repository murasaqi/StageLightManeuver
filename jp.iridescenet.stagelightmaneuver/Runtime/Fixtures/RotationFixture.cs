using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class RotationFixture:StageLightFixtureBase
    {
        public Transform target;
        public Vector3 rotationAxis = new Vector3(0,0,1);
        [FormerlySerializedAs("rotationScalar")] public float rotationSpeed = 0f;


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

            // rotationAxis = Vector3.zero;
            rotationSpeed = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var rotationProperty = queueData.TryGet<RotationProperty>() as RotationProperty;

                if (rotationProperty == null || stageLightBaseProperties == null)
                    return;

                var normalizedTime = GetNormalizedTime(time, queueData, typeof(RotationProperty));

                // rotationAxis += rotationProperty.rotationAxis.value * queueData.weight;
                rotationSpeed += rotationProperty.rotationSpeed.value.Evaluate(normalizedTime)*time * queueData.weight;

            }
            
            rotationSpeed = rotationSpeed % 360;
        }

        public override void UpdateFixture()
        {
            
            if(target) target.localEulerAngles = rotationAxis * rotationSpeed;
        }
    }
    
    
}