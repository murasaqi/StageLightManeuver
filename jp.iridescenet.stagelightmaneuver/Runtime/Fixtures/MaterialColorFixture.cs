using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace StageLightManeuver
{
    [ExecuteAlways]
    public class MaterialColorFixture : StageLightFixtureBase
    {
        public LightProperty lightProperty = new LightProperty();
        // public StageLightProperty<bool> fromLightFixture = new StageLightProperty<bool>();
        public MeshRenderer meshRenderer;
        public SlmToggleValue<string> materialToggleValueName = new SlmToggleValue<string>(){value = "_Color"};
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
        public override void UpdateFixture(float currentTime)
        {
            if(meshRenderer == null || _materialPropertyBlock == null) return;
            base.UpdateFixture(currentTime);

        }

        private void Update()
        {
           
            meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }

}
