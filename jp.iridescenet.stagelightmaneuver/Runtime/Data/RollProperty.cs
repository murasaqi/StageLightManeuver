using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
    [Serializable]
    public class MinMaxEasingValue
    {
        [DisplayName("Mode")] public AnimationMode mode = AnimationMode.Ease;
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
            var value = 0f;
            if (mode == AnimationMode.AnimationCurve)
            {
                value = animationCurve.Evaluate(t);
            }
            else if (mode == AnimationMode.Ease)
            {
                value = EaseUtil.GetEaseValue(easeType, t, 1f, valueRange.x,
                    valueRange.y);
            }
            else if (mode == AnimationMode.Constant)
            {
                value = constant;
            }

            return value;
        }
    }
    [Serializable]
    public class RollProperty:SlmAdditionalProperty
    {
        [DisplayName("Roll Transform")]public SlmToggleValue<MinMaxEasingValue> rollTransform;
       
        public RollProperty(RollProperty rollProperty)
        {
            propertyName = rollProperty.propertyName;

            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = rollProperty.bpmOverrideData.propertyOverride,
                value = new BpmOverrideToggleValueBase()
                {
                    bpmOverride = rollProperty.bpmOverrideData.value.bpmOverride,
                    bpmScale = rollProperty.bpmOverrideData.value.bpmScale,
                    childStagger = rollProperty.bpmOverrideData.value.childStagger,
                    loopType = rollProperty.bpmOverrideData.value.loopType,
                    offsetTime = rollProperty.bpmOverrideData.value.offsetTime,
                    propertyOverride = rollProperty.bpmOverrideData.value.propertyOverride,
                }
            };
            this.rollTransform = new SlmToggleValue<MinMaxEasingValue>()
            {
                value =     new MinMaxEasingValue(rollProperty.rollTransform.value.mode, rollProperty.rollTransform.value.valueRange, rollProperty.rollTransform.value.valueMinMax, rollProperty.rollTransform.value.easeType, rollProperty.rollTransform.value.constant, rollProperty.rollTransform.value.animationCurve),
            };
            propertyOverride = rollProperty.propertyOverride;

            // this.animationCurve = rollProperty.animationCurve;
        }

        public RollProperty()
        {
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            rollTransform = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            rollTransform.propertyOverride = toggle;
            
        }
    }
    
    [Serializable]
    public class PanProperty:RollProperty
    {
        public PanProperty(PanProperty panProperty):base(panProperty)
        {
            propertyName = "Pan";
        }

        public PanProperty():base()
        {
            propertyName = "Pan";
        }
    }
    [Serializable]
    public class TiltProperty:RollProperty
    {
        public TiltProperty(TiltProperty tiltProperty):base(tiltProperty)
        {
            
            propertyName = "Tilt";
        }

        public TiltProperty():base()
        {
            
            propertyName = "Tilt";
        }
    }

}