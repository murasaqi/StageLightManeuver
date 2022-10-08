using System;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class GoboFixture:StageLightFixtureBase
    
    {
        public MeshRenderer meshRenderer;
        public Texture2D goboTexture;
        public string goboPropertyName = "_GoboTexture";
        private MaterialPropertyBlock _materialPropertyBlock;
        
        public Transform goboTransform;
        public float speed = 0f;
        public Vector3 goboRotateVector = new Vector3(0, 0, 1);
        
        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        [ContextMenu("Init")]
        public override void Init()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
        }

        public override void UpdateFixture(float time)
        {
            goboTexture = null;
            speed = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var goboProperty = queueData.TryGet<GoboProperty>() as GoboProperty;
                
                if(goboProperty == null || stageLightBaseProperties == null)continue;
                var bpm = stageLightBaseProperties.bpm.value;
                var bpmOffset = goboProperty.bpmOverrideData.value.bpmOverride
                    ? goboProperty.bpmOverrideData.value.bpmOffset
                    : stageLightBaseProperties.bpmOffset.value;
                var bpmScale = goboProperty.bpmOverrideData.value.bpmOverride
                    ? goboProperty.bpmOverrideData.value.bpmScale
                    : stageLightBaseProperties.bpmScale.value;
                var loopType = goboProperty.bpmOverrideData.value.bpmOverride
                    ? goboProperty.bpmOverrideData.value.loopType
                    : stageLightBaseProperties.loopType.value;
                var clipProperty = stageLightBaseProperties.clipProperty;
                var t =GetNormalizedTime(time,
                    bpm,
                    bpmOffset,
                    bpmScale, 
                    clipProperty,
                    loopType);
                
                
                if(goboProperty ==null || stageLightBaseProperties == null) continue;
                if (queueData.weight > 0.5f)
                {
                    goboTexture = goboProperty.goboTexture.value;
                    goboPropertyName = goboProperty.goboPropertyName.value;
                }

                if (goboProperty.goroRotationSpeed.value.mode == AnimationMode.Ease)
                {
                    speed +=EaseUtil.GetEaseValue(goboProperty.goroRotationSpeed.value.easeType, time, 1f, goboProperty.goroRotationSpeed.value.rollRange.x,
                        goboProperty.goroRotationSpeed.value.rollRange.y) * queueData.weight;
                    
                }
                else
                {
                    speed += goboProperty.goroRotationSpeed.value.animationCurve.Evaluate(t) * queueData.weight;     
                }
               
            }
        }

        private void Update()
        {
            
            if (goboTransform != null)
            {
                goboTransform.localEulerAngles += goboRotateVector *(speed*(float)Time.deltaTime);
            }
            if (meshRenderer != null)
            {
                if (_materialPropertyBlock == null)
                {
                    Init();
                    if(_materialPropertyBlock == null) return;
                }
                
                if (goboTexture != null)
                {
                    _materialPropertyBlock.SetTexture(goboPropertyName,goboTexture);
                }
                else
                {
                    _materialPropertyBlock.SetTexture(goboPropertyName,Texture2D.whiteTexture);
                }
                meshRenderer.SetPropertyBlock(_materialPropertyBlock);
            }

           
           
        }
    
    }
}