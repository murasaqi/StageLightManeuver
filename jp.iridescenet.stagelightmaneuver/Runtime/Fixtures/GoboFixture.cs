using System;
using UnityEngine;

using VLB;
using Random = UnityEngine.Random;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class GoboFixture:StageLightFixtureBase
    
    {
#if USE_VLB_ALTER
        public VolumetricLightBeam volumetricLightBeam;
        
#endif
        public MeshRenderer meshRenderer;
        public Texture2D goboTexture;
        public string goboPropertyName = "_GoboTexture";
        private MaterialPropertyBlock _materialPropertyBlock;
        
        public Transform goboTransform;
        public float speed = 0f;
        public Vector3 goboRotateVector = new Vector3(0, 0, 1);
        
        public bool rotateStartOffsetRandom = false;
        // private float _rotateStartOffset = 0f;
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
            goboTransform.localEulerAngles = goboRotateVector * (rotateStartOffsetRandom ? Random.Range(0f, 360f) : 0f);
            _materialPropertyBlock = new MaterialPropertyBlock();
            
#if USE_VLB_ALTER
            if (volumetricLightBeam)
            {
                volumetricLightBeam.RegisterOnBeamGeometryInitializedCallback(() =>
                {
                    var beamGeometry = volumetricLightBeam.transform.GetChild(0).GetComponent<MeshRenderer>(); 
                    meshRenderer = beamGeometry;
                    if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
                });     
            }
           
#else
             if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
#endif
            
           

        }

        public override void EvaluateQue(float time)
        {
            goboTexture = null;
            speed = 0f;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var goboProperty = queueData.TryGet<GoboProperty>() as GoboProperty;
                
                if(goboProperty == null || stageLightBaseProperties == null)continue;

                var t = GetNormalizedTime(time, queueData, typeof(GoboProperty));


                if(goboProperty ==null || stageLightBaseProperties == null) continue;
                if (queueData.weight > 0.5f)
                {
                    goboTexture = goboProperty.goboTexture.value;
                    goboPropertyName = goboProperty.goboPropertyName.value;
                }

                if (goboProperty.goroRotationSpeed.value.mode == AnimationMode.Ease)
                {
                    speed +=EaseUtil.GetEaseValue(goboProperty.goroRotationSpeed.value.easeType, time, 1f, goboProperty.goroRotationSpeed.value.valueRange.x,
                        goboProperty.goroRotationSpeed.value.valueRange.y) * queueData.weight;
                    
                }
                else if(goboProperty.goroRotationSpeed.value.mode == AnimationMode.AnimationCurve)
                {
                    speed += goboProperty.goroRotationSpeed.value.animationCurve.Evaluate(t) * queueData.weight;     
                }
                else if(goboProperty.goroRotationSpeed.value.mode == AnimationMode.Constant)
                {
                    speed += goboProperty.goroRotationSpeed.value.constant * queueData.weight;     
                }
               
            }
        }

        public override void UpdateFixture()
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