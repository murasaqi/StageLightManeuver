using System;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public enum LightTransformType
    {
        Pan,
        Tilt
    }
    
    public enum AnimationMode
    {
        Ease,
        AnimationCurve,
        Constant
    }
    
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class LightPanFixture: StageLightFixtureBase
    {
        private LightTransformType _lightTransformType = LightTransformType.Pan;
        private float _angle;
        public Vector3 rotationVector = Vector3.up;
        public Transform rotateTransform;


        public LightTransformType LightTransformType => _lightTransformType;

        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        public override void Init()
        {
            PropertyType = typeof(PanProperty);
            // rotationVector = _lightTransformType == LightTransformType.Pan ? Vector3.up : Vector3.left;
        }

        public override void EvaluateQue(float currentTime)
        {   
            base.EvaluateQue(currentTime);
            if(rotateTransform == null) return;
           
            _angle = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();

                var qLightBaseProperty = queueData.TryGet<TimeProperty>() as TimeProperty;
                var qPanProperty = queueData.TryGet<PanProperty>() as PanProperty;
                var weight = queueData.weight;
                if (qPanProperty == null || qLightBaseProperty == null) continue;
              
                var normalizedTime = GetNormalizedTime(currentTime,queueData,typeof(PanProperty));
                // Debug.Log($"{queueData.stageLightSetting.name},{time}");
                
                var manualPanTiltProperty = queueData.TryGet<ManualPanTiltProperty>();
               
                if(manualPanTiltProperty != null)
                {
                    var positions = manualPanTiltProperty.positions.value;
                    var mode = manualPanTiltProperty.mode.value;
                    if (Index < positions.Count)
                    {
                       switch (mode)
                        {
                            case ManualPanTiltMode.Overwrite:
                                _angle += positions[Index].pan * weight;
                                break;
                            case ManualPanTiltMode.Add:
                                _angle += (positions[Index].pan+qPanProperty.rollTransform.value.Evaluate(normalizedTime)) * weight;
                                break;
                        }
                            // Debug.Log($"pan({Index}): {positions[Index].pan}, weight: {weight}");
                    }
                    
                }
                else
                {
                    
                
                    _angle += qPanProperty.rollTransform.value.Evaluate(normalizedTime) * weight;
                     
                }
                
                
               
                
            }
            
        }

        public override void UpdateFixture()
        {
            rotateTransform.localEulerAngles =  rotationVector * _angle;
        }
    }
}