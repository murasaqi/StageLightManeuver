using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class MinMaxEasingValue
    {
        [DisplayName("Mode")] public AnimationMode mode = AnimationMode.Ease;
        [DisplayName("Inverse")] public bool inverse = false;
        [DisplayName("Range")]public Vector2 valueRange = new Vector2(0, 0);
        public Vector2 valueMinMax = new Vector2(-180, 180);
        [DisplayName("Easing")]public EaseType easeType = EaseType.Linear;
        [DisplayName("Constant")]public float constant = 0;
        [DisplayName("Curve")]public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0,0),
            new Keyframe(1,1)
        });


        public MinMaxEasingValue()
        {
            mode = AnimationMode.Ease;
            valueRange = new Vector2(0, 0);
            valueMinMax = new Vector2(-180, 180);
            easeType = EaseType.Linear;
            constant = 0;
            animationCurve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0,0),
                new Keyframe(1,1)
            });
        }
        
        public MinMaxEasingValue(MinMaxEasingValue other)
        {
            mode = other.mode;
            valueRange = new Vector2( other.valueRange.x, other.valueRange.y);
            valueMinMax = new Vector2( other.valueMinMax.x, other.valueMinMax.y);
            easeType = other.easeType;
            constant = other.constant;
            animationCurve = SlmUtility.CopyAnimationCurve(other.animationCurve);
        }
        
        public MinMaxEasingValue(AnimationMode mode, Vector2 rollRange, Vector2 rollMinMax, EaseType easeType, float constant, AnimationCurve animationCurve)
        {
            this.mode = mode;
            this.valueRange = new Vector2( rollRange.x, rollRange.y);
            this.valueMinMax = new Vector2( rollMinMax.x, rollMinMax.y);
            this.easeType = easeType;
            this.constant = constant;
            var keys = new List<Keyframe>();
            
            foreach (var keyframe in animationCurve.keys)
            {
                keys.Add(new Keyframe(keyframe.time, keyframe.value));
            }
            this.animationCurve = new AnimationCurve(keys.ToArray());
        }

        public float Evaluate(float t)
        {
            var time = inverse ? 1f - t : t;
            var value = 0f;
            if (mode == AnimationMode.AnimationCurve)
            {
                value = animationCurve.Evaluate(time);
            }
            else if (mode == AnimationMode.Ease)
            {
                value = EaseUtil.GetEaseValue(easeType, time, 1f, valueRange.x,
                    valueRange.y);
            }
            else if (mode == AnimationMode.Constant)
            {
                value = constant;
            }

            return value;
        }
    }
}