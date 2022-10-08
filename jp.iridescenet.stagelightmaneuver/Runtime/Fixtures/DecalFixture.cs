using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class DecalFixture: StageLightFixtureBase
    {
        public LightFixture lightFixture;
        public Texture2D decalTexture;
        public Color decalColor = Color.white;
        public float decalSizeScaler = 1f;
        public float floorHeight = 0f;
        public float decalDepthScaler = 1f;
        public float fadeFactor = 1f;
        public float opacity = 1f;
        public DecalProjector decalProjector;
        public Material decalMaterial;
        private Material _instancedDecalMaterial = null;
        private float _radius = 1f;
        private float _depth = 1f;
        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }
        
        public override void UpdateFixture(float time)
        {
            if(decalMaterial == null || decalProjector == null || decalProjector.material == null) return;
            
            opacity = 0f;
            fadeFactor = 0f;
            decalSizeScaler = 0f;
            decalDepthScaler = 0f;
            floorHeight = 0f;
            
            
            while (stageLightDataQueue.Count >0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var qDecalProperty = queueData.TryGet<DecalProperty>() as DecalProperty;
                if (qDecalProperty == null || stageLightBaseProperties == null) continue;
                var weight = queueData.weight;
                var bpm = stageLightBaseProperties.bpm.value;
                var bpmOffset = qDecalProperty.bpmOverrideData.value.bpmOverride
                    ? qDecalProperty.bpmOverrideData.value.bpmOffset
                    : stageLightBaseProperties.bpmOffset.value;
                var bpmScale = qDecalProperty.bpmOverrideData.value.bpmOverride
                    ? qDecalProperty.bpmOverrideData.value.bpmScale
                    : stageLightBaseProperties.bpmScale.value;
                var loopType = qDecalProperty.bpmOverrideData.value.bpmOverride
                    ? qDecalProperty.bpmOverrideData.value.loopType
                    : stageLightBaseProperties.loopType.value;
                var t = GetNormalizedTime(
                    time,
                    bpm,
                    bpmOffset,
                    bpmScale,
                    stageLightBaseProperties.clipProperty,
                    loopType);
                
                opacity += qDecalProperty.opacity.value * weight;
                fadeFactor += qDecalProperty.fadeFactor.value * weight;
                decalSizeScaler += qDecalProperty.decalSizeScaler.value * weight;
                decalDepthScaler += qDecalProperty.decalDepthScaler.value * weight;
                floorHeight += qDecalProperty.floorHeight.value * weight;
                if(weight > 0.5f)decalTexture = qDecalProperty.decalTexture.value;
            }

            decalColor = lightFixture.lightColor;

        }

        private void Update()
        {
            if(decalProjector ==null) return;
            
            var floor = new Vector3(0,floorHeight,0);
            var distance = Vector3.Distance(transform.position,floor);
            var angle = lightFixture.spotAngle;
            _radius = Mathf.Tan(angle * Mathf.Deg2Rad) * distance * decalSizeScaler;
            _depth = distance * decalDepthScaler;
            
            decalProjector.size = new Vector3(_radius,_radius, _depth);
            decalProjector.fadeFactor = fadeFactor;
            if (lightFixture != null) decalProjector.fadeFactor *= lightFixture.lightIntensity; 
            decalProjector.pivot = new Vector3(0, 0, _depth / 2f);
            
            decalProjector.material.SetFloat("_Alpha",opacity*
                                                      Vector3.Distance(Vector3.zero, new Vector3(
                                                          Mathf.Clamp(decalColor.r,0,1f), 
                                                          Mathf.Clamp(decalColor.g,0,1), 
                                                          Mathf.Clamp(decalColor.b,0,1))));
            decalProjector.material.SetColor("_Color",decalColor);
            decalProjector.material.SetTexture("_MainTex",decalTexture);
        }
        public override void Init()
        {
            if(_instancedDecalMaterial != null) DestroyImmediate(_instancedDecalMaterial);
            if (decalProjector != null)
            {
                _instancedDecalMaterial = Material.Instantiate(decalMaterial);
                decalProjector.material = _instancedDecalMaterial;     
            }
            
            lightFixture = GetComponent<LightFixture>();
        }
        
        
    }
}
