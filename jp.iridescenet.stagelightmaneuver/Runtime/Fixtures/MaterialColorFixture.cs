using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StageLightManeuver
{
    
    public class MaterialColorFixture:StageLightFixtureBase
    {
        public MeshRenderer meshRenderer;
        public List<MeshRenderer> syncMeshRenderers = new List<MeshRenderer>();
        public int materialIndex;
        private MaterialPropertyBlock _materialPropertyBlock;
        [SerializeField] private float intensity = 1;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private string colorPropertyName = "_MainColor";
        private Dictionary<MeshRenderer,MaterialPropertyBlock> _materialPropertyBlocks = null;
        private int propertyId;
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        [ContextMenu("Get Mesh Renderer")]
        public void GetMeshRenderer()
        {
            meshRenderer = GetComponent<MeshRenderer>();
         
        }

        public override void Init()
        {
            
            _materialPropertyBlock = new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlocks = new Dictionary<MeshRenderer, MaterialPropertyBlock>();
            foreach (var meshRenderer in syncMeshRenderers)
            {
                if(meshRenderer == null) continue;
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlocks.Add(meshRenderer,materialPropertyBlock);
            }
        }

        private void OnValidate()
        {
            Init();
        }

        public override void EvaluateQue(float currentTime)
        {
            color = new Color(0, 0, 0, 0);
            intensity = 0;

            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var timeProperty = queueData.TryGet<TimeProperty>();
                var materialProperty = queueData.TryGet<MaterialColorProperty>();
                var weight = queueData.weight;
                if (timeProperty == null || materialProperty == null)
                {
                    return;
                };
                if (weight >= 0.5f)
                {
                    colorPropertyName = materialProperty.colorPropertyName.value;
                    if (materialIndex != materialProperty.materialindex.value)
                    {
                        materialIndex = materialProperty.materialindex.value;
                        Init();
                    }
                }
                var t = GetNormalizedTime(currentTime, queueData, typeof(MaterialColorProperty));
                color += materialProperty.color.value.Evaluate(t) * weight;
                intensity += materialProperty.intensity.value.Evaluate(t) * weight;
                
                
            }
        }

        public override void UpdateFixture()
        {
            
            if (_materialPropertyBlock == null || _materialPropertyBlocks == null) return;
            {
                Init();
            }
            
            if(_materialPropertyBlock ==null) return;
            
            _materialPropertyBlock.SetColor(colorPropertyName,SlmUtility.GetHDRColor(color,intensity));
            if(meshRenderer)meshRenderer.SetPropertyBlock(_materialPropertyBlock,materialIndex);
            
            foreach (var materialPropertyBlock in _materialPropertyBlocks)
            {
                var meshRenderer = materialPropertyBlock.Key;
                var block = materialPropertyBlock.Value;
                if(meshRenderer == null || block == null) continue;
                block.SetColor(colorPropertyName,SlmUtility.GetHDRColor(color,intensity));
                meshRenderer.SetPropertyBlock(block,materialIndex);
            }
        }
    }
}