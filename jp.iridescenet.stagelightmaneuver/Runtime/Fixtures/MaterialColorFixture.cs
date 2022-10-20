using System;
using UnityEngine;

namespace StageLightManeuver
{
    public class MaterialColorFixture:StageLightFixtureBase
    {
        public MeshRenderer meshRenderer;
        public int materialIndex;
        private MaterialPropertyBlock _materialPropertyBlock;
        [SerializeField] private float intensity = 1;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private string colorPropertyName = "_MainColor";
        private int propertyId;
        void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        public override void Init()
        {
            
            _materialPropertyBlock = new MaterialPropertyBlock();
            if(meshRenderer)meshRenderer.GetPropertyBlock(_materialPropertyBlock);
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
            
            if (_materialPropertyBlock == null)
            {
                Init();
            }
            
            if(_materialPropertyBlock ==null) return;
            
            _materialPropertyBlock.SetColor(colorPropertyName,SlmUtility.GetHDRColor(color,intensity));
            meshRenderer.SetPropertyBlock(_materialPropertyBlock,materialIndex);
        }
    }
}