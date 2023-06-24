
#if USE_VLB
using System.Collections;
using UnityEngine;

namespace VLB
{
    [HelpURL(Consts.Help.UrlEffectPulse)]
 [ExecuteAlways]
    public class VLBSideThicknessAutoModifier : EffectAbstractBase
    {
        public new const string ClassName = "VLBSideThicknessModifier";
        
        
        public AnimationCurve thicknessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        // [Range(0f, 0.999f)]
        // public float thicknessMin = 0f;
        //
        // [Range(0.001f,1f)]
        // public float thicknessMax = 1f;

        private const float fresnelPowMax = 10f;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(CoUpdate());
        }

        IEnumerator CoUpdate()
        {
          
            var t = 0.0f;
            while (true)
            {
                if (m_Light != null)
                {
                    var angleDiff = Mathf.Clamp(Mathf.Max(m_Light.spotAngle - m_Light.innerSpotAngle,0)/ 180f,0f,1f);
                    m_Beam.fresnelPow = Mathf.Lerp(0, fresnelPowMax, thicknessCurve.Evaluate(angleDiff));
                }

                yield return null;
                t += Time.deltaTime;
            }
        }
    }
}
#endif