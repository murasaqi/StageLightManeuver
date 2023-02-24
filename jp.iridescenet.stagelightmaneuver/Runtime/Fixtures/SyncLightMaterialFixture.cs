using System;
using System.Collections.Generic;
using UnityEngine;

#if USE_HDRP
using UnityEngine.Rendering.HighDefinition;
#elif USE_URP
using UnityEngine.Rendering.Universal;
#endif



namespace StageLightManeuver
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class SyncLightMaterialFixture : StageLightFixtureBase
    {
        // public StageLightProperty<bool> fromLightFixture = new StageLightProperty<bool>();
        public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
#if USE_HDRP
        public string materialPropertyName =  "_EmissiveColor";
#elif USE_URP
        public string materialPropertyName =  "_EmissionColor";
#endif
        
        public float intensityMultiplier = 1f;
        public bool brightnessDecreasesToBlack = true;
        private Dictionary<MeshRenderer,MaterialPropertyBlock> _materialPropertyBlocks;
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
            if(_materialPropertyBlocks != null) _materialPropertyBlocks.Clear();
            _materialPropertyBlocks = new Dictionary<MeshRenderer, MaterialPropertyBlock>();
            // if(meshRenderers)meshRenderer.GetPropertyBlock(_materialPropertyBlock);

            foreach (var meshRenderer in meshRenderers)
            {
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlocks.Add(meshRenderer,materialPropertyBlock);
            }
        }
        public override void EvaluateQue(float currentTime)
        {
            if(meshRenderers == null || _materialPropertyBlocks == null) return;

            intensityMultiplier = 0f;
            while (stageLightDataQueue.Count>0)
            {
                
                var data = stageLightDataQueue.Dequeue();
                // var t=GetNormalizedTime(currentTime,data,typeof(SyncLightMaterialProperty));

                var syncLightMaterialProperty = data.TryGet<SyncLightMaterialProperty>();
                if(syncLightMaterialProperty != null)
                {
                    intensityMultiplier += syncLightMaterialProperty.intensitymultiplier.value * data.weight;
                    if(data.weight > 0.5f)
                    {
                        brightnessDecreasesToBlack = syncLightMaterialProperty.brightnessDecreasesToBlack.value;
                    }
                }

            }
           
            base.EvaluateQue(currentTime);

        }

        public override void UpdateFixture()
        {
            if(lightFixture == null) return;
            if (_materialPropertyBlocks == null|| _materialPropertyBlocks.Count != meshRenderers.Count)
            {
                Init();
            }
            
            var intensity = lightFixture.lightIntensity * intensityMultiplier;
            var hdrColor = SlmUtility.GetHDRColor(lightFixture.lightColor,
                intensity);
            var result = brightnessDecreasesToBlack ? Color.Lerp(Color.black,hdrColor, Mathf.Clamp(intensity, 0, 1f)) : hdrColor;

            foreach (var materialPropertyBlock in _materialPropertyBlocks)
            {
                materialPropertyBlock.Value.SetColor(materialPropertyName,result);
                materialPropertyBlock.Key.SetPropertyBlock(materialPropertyBlock.Value);
            }
        }
    }

}
