using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace StageLightManeuver
{
    [ExecuteAlways]
    public class SyncLightMaterialFixture : StageLightFixtureBase
    {
        // public StageLightProperty<bool> fromLightFixture = new StageLightProperty<bool>();
        public MeshRenderer meshRenderer;
        public string materialPropertyName =  "_EmissionColor";
        public float intensityMultiplier = 1f;
        private MaterialPropertyBlock _materialPropertyBlock;
        public LightFixture lightFixture;
        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init(); 
            lightFixture = GetComponent<LightFixture>();
        }

        public override void Init()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
        }
        public override void EvaluateQue(float currentTime)
        {
            if(meshRenderer == null || _materialPropertyBlock == null) return;

            intensityMultiplier = 0f;
            while (stageLightDataQueue.Count>0)
            {
                
                var data = stageLightDataQueue.Dequeue();
                // var t=GetNormalizedTime(currentTime,data,typeof(SyncLightMaterialProperty));

                var syncLightMaterialProperty = data.TryGet<SyncLightMaterialProperty>();
                if(syncLightMaterialProperty != null)
                {
                    intensityMultiplier += syncLightMaterialProperty.intensitymultiplier.value * data.weight;
                }

            }
           
            base.EvaluateQue(currentTime);

        }

        public override void UpdateFixture()
        {
            if(lightFixture == null) return;
            if (_materialPropertyBlock == null)
            {
                Init();
            }
            
            if(_materialPropertyBlock ==null) return;
            
           _materialPropertyBlock.SetColor(materialPropertyName,SlmUtility.GetHDRColor(lightFixture.lightColor,lightFixture.lightIntensity*intensityMultiplier));
            meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }

}
