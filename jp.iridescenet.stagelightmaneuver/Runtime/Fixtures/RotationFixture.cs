﻿using System;
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
        private float rotation = 0f;


        private void Start()
        {
            Init();
        }
        
        public override void Init()
        {
            base.Init();
            PropertyTypes.Add(typeof(RotationProperty));       
        }
        

        public override void EvaluateQue(float time)
        {

            // rotationAxis = Vector3.zero;
            // rotationSpeed = 0f;
            rotationSpeed = 0f;
            rotation = 0f;
            var offsetTime = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var rotationProperty = queueData.TryGet<RotationProperty>() as RotationProperty;

                if (rotationProperty == null || stageLightBaseProperties == null)
                    return;

                var normalizedTime = GetNormalizedTime(time, queueData, typeof(RotationProperty));
                offsetTime += GetOffsetTime(queueData, typeof(RotationProperty)) * queueData.weight;

                // rotationAxis += rotationProperty.rotationAxis.value * queueData.weight;
                rotationSpeed += rotationProperty.rotationSpeed.value.Evaluate(normalizedTime) * queueData.weight;
                // Debug.Log($"{Index}: {(time * offsetTime)}, {rotationSpeed}");
                // rotation += rotationSpeed * (time * offsetTime) * queueData.weight;

            }

            rotation = (rotationSpeed * (time + offsetTime)) % 360;
            // rotationSpeed = rotationSpeed % 360;
        }

        public override void UpdateFixture()
        {
            
            if(target) target.localEulerAngles = rotationAxis * rotation;
        }
    }
    
    
}