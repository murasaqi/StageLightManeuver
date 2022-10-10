using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class LightTiltFixture: StageLightFixtureBase
    {
        private LightTransformType _lightTransformType = LightTransformType.Tilt;
        private float _angle =0f;
        public Transform rotateTransform;
        public override void EvaluateQue(float currentTime)
        {
            base.EvaluateQue(currentTime);
            if(rotateTransform == null) return;
            _angle = 0f;
            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var qTiltProperty = queueData.TryGet<TiltProperty>() as TiltProperty;
                var stageLightBaseProperty = queueData.TryGet<TimeProperty>() as TimeProperty;
                var weight = queueData.weight;
                if (qTiltProperty == null || stageLightBaseProperty == null) continue;
                var bpm = stageLightBaseProperty.bpm.value;
                var bpmOffset = qTiltProperty.bpmOverrideData.value.bpmOverride ? qTiltProperty.bpmOverrideData.value.bpmOffset : stageLightBaseProperty.bpmOffset.value;
                var bpmScale = qTiltProperty.bpmOverrideData.value.bpmOverride ? qTiltProperty.bpmOverrideData.value.bpmScale : stageLightBaseProperty.bpmScale.value;
                var loopType = qTiltProperty.bpmOverrideData.value.bpmOverride ? qTiltProperty.bpmOverrideData.value.loopType : stageLightBaseProperty.loopType.value;
                var clipProperty = stageLightBaseProperty.clipProperty;
                var time = GetNormalizedTime(currentTime,bpm,bpmOffset,bpmScale,clipProperty,loopType);
               
                // var end = qTiltProperty.endRoll.value;
                var manualPanTiltProperty = queueData.TryGet<ManualPanTiltProperty>();
                if(manualPanTiltProperty != null)
                {
                    var positions = manualPanTiltProperty.positions.value;
                    if (Index < positions.Count)
                    {
                        _angle += positions[Index].pan * weight;
                    }
                    
                }
                else
                {
                    if (qTiltProperty.rollTransform.value.mode == AnimationMode.Ease)
                    {
                        _angle += EaseUtil.GetEaseValue(qTiltProperty.rollTransform.value.easeType, time, 1f, qTiltProperty.rollTransform.value.rollRange.x, qTiltProperty.rollTransform.value.rollRange.y) * weight;
                        // if(weight >= 1f)Debug.Log($"{queueData.stageLightSetting.name}: {_angle},{time},{weight}");

                    }
                    if (qTiltProperty.rollTransform.value.mode == AnimationMode.AnimationCurve)
                    {
                        _angle += qTiltProperty.rollTransform.value.animationCurve.Evaluate(time) * weight;
                    }
                    if (qTiltProperty.rollTransform.value.mode == AnimationMode.Constant)
                    {
                        _angle += qTiltProperty.rollTransform.value.constant * weight;
                    }       
                }
             
               

            }
        }
        
        public LightTransformType LightTransformType => _lightTransformType;
        
        public override void UpdateFixture()
        {
            var vec = _lightTransformType == LightTransformType.Pan ? Vector3.up : Vector3.left;
            rotateTransform.localEulerAngles =  vec * _angle;
        }
    }
}