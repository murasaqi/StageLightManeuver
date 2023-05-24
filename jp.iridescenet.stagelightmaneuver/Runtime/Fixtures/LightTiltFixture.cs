using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class LightTiltFixture: StageLightFixtureBase
    {
        // private LightTransformType _lightTransformType = LightTransformType.Tilt;
        private float _angle =0f;
        public Vector3 rotationVector = Vector3.left;
        public Transform rotateTransform;
        public bool ignore = false;
        private Vector3 currentVelocity;
        public float smoothTime = 0.05f;
        private float maxSpeed = float.PositiveInfinity;
        [FormerlySerializedAs("smoothness")] public bool useSmoothness = false;
        private float previousAngle = 0f;
        public float minAngleValue = -360;
        public float maxAngleValue = 360;
        
        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            if(rotateTransform == null) return;
            _angle = 0f;
            smoothTime = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var qTiltProperty = queueData.TryGetActiveProperty<TiltProperty>() as TiltProperty;
                var timeProperty = queueData.TryGetActiveProperty<ClockProperty>() as ClockProperty;
                var weight = queueData.weight;
                if (qTiltProperty == null || timeProperty == null) continue;
                var normalizedTime = SlmUtility.GetNormalizedTime(currentTime, queueData, typeof(TiltProperty), Index);
                var manualPanTiltProperty = queueData.TryGetActiveProperty<ManualPanTiltProperty>();
                
                var lookAtProperty = queueData.TryGetActiveProperty<LookAtProperty>();
                ignore = lookAtProperty != null;
                
                if(manualPanTiltProperty != null)
                {
                    var positions = manualPanTiltProperty.positions.value;
                    var mode = manualPanTiltProperty.mode.value;
                    if (Index < positions.Count)
                    {
                        switch (mode)
                        {
                            case ManualPanTiltMode.Overwrite:
                                _angle += positions[Index].tilt * weight;
                                break;
                            case ManualPanTiltMode.Add:
                                _angle += (positions[Index].tilt+qTiltProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                        }
                        // Debug.Log($"tilt({Index}): {positions[Index].tilt}, weight: {weight}");
                    }
                }
                else
                {
                    _angle += qTiltProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
                }
                
                smoothTime += qTiltProperty.smoothTime.value * weight;
                if(weight > 0.5f) useSmoothness = qTiltProperty.useSmoothness.value;

            }
            
            _angle = Mathf.Clamp(_angle, minAngleValue, maxAngleValue);
        }
        
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        public override void UpdateFixture()
        {
            if(ignore) return;
            
            if(useSmoothness) return;
            rotateTransform.localEulerAngles = rotationVector * _angle;
            
        }

        public void Update()
        {
            if (useSmoothness)
            {
                var smoothAngle = Mathf.SmoothDampAngle(previousAngle, _angle, ref currentVelocity.x, smoothTime, maxSpeed);
                rotateTransform.localEulerAngles = rotationVector * smoothAngle;
                previousAngle = smoothAngle;
            }
            
           
        }

        public override void Init()
        {
            PropertyTypes.Add(typeof(TiltProperty));
        }
    }
}