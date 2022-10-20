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
    public class LightPanFixture: StageLightFixtureBase
    {
        private LightTransformType _lightTransformType = LightTransformType.Pan;
        private float _angle;
        public Transform rotateTransform;


        public LightTransformType LightTransformType => _lightTransformType;
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
              
                var time = GetNormalizedTime(currentTime,queueData,typeof(PanProperty));
                // Debug.Log($"{queueData.stageLightSetting.name},{time}");
                
                var manualPanTiltProperty = queueData.TryGet<ManualPanTiltProperty>();
                
                if(manualPanTiltProperty != null)
                {
                    var positions = manualPanTiltProperty.positions.value;
                    if (Index < positions.Count)
                    {
                        _angle += positions[Index].tilt * weight;
                    }
                    
                }
                else
                {
                    if (qPanProperty.rollTransform.value.mode == AnimationMode.Ease)
                    {
                        _angle += EaseUtil.GetEaseValue(qPanProperty.rollTransform.value.easeType, time, 1f, qPanProperty.rollTransform.value.rollRange.x,
                            qPanProperty.rollTransform.value.rollRange.y) * weight;
                    }
                    if (qPanProperty.rollTransform.value.mode == AnimationMode.AnimationCurve)
                    {
                        _angle += qPanProperty.rollTransform.value.animationCurve.Evaluate(time) * weight;
                    }
                    if (qPanProperty.rollTransform.value.mode == AnimationMode.Constant)
                    {
                        _angle += qPanProperty.rollTransform.value.constant * weight;
                    }    
                }
                
                
               
                
            }
            
        }

        public override void UpdateFixture()
        {
            var vec = _lightTransformType == LightTransformType.Pan ? Vector3.up : Vector3.left;
            rotateTransform.localEulerAngles =  vec * _angle;
        }
    }
}