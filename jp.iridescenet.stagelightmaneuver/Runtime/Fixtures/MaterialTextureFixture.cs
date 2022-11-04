﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    public class MaterialTextureFixture:StageLightFixtureBase
    {
        public List<MeshRenderer> meshRenderers;
        public int materialIndex;
        public Texture2D texture2D;
        public string propertyName;
        private Dictionary<MeshRenderer,MaterialPropertyBlock> _materialPropertyBlockDictionary = new Dictionary<MeshRenderer, MaterialPropertyBlock>();

        private void Start()
        {
            Init();
        }

        protected void OnEnable()
        {
            Init();
        }

        public override void Init()
        {
            _materialPropertyBlockDictionary = new Dictionary<MeshRenderer, MaterialPropertyBlock>();
            foreach (var meshRenderer in meshRenderers)
            {
                if(meshRenderer == null) continue;
                var materialPropertyBlock = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlockDictionary.Add(meshRenderer,materialPropertyBlock);
            }
        }
        
        private void OnValidate()
        {
            Init();
        }
        
         public override void EvaluateQue(float currentTime)
         {
             texture2D = Texture2D.whiteTexture;

            while (stageLightDataQueue.Count>0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var timeProperty = queueData.TryGet<TimeProperty>();
                var materialTextureProperty = queueData.TryGet<MaterialTextureProperty>();
                var weight = queueData.weight;
                if (timeProperty == null || materialTextureProperty == null)
                {
                    return;
                };
                if (weight >= 0.5f)
                {
                    texture2D = materialTextureProperty.texture.value;
                    propertyName = materialTextureProperty.texturePropertyName.value;
                    if (materialIndex != materialTextureProperty.materialindex.value)
                    {
                        materialIndex = materialTextureProperty.materialindex.value;
                        Init();
                    }
                }
              
                
            }
        }

        public override void UpdateFixture()
        {
            
            if (_materialPropertyBlockDictionary == null )
            {
                Init();
            }

            foreach (var materialProperty in _materialPropertyBlockDictionary)
            {
                var meshRenderer = materialProperty.Key;
                var materialPropertyBlock = materialProperty.Value;
                
                if(meshRenderer ==null || materialPropertyBlock == null) continue;
                
                materialPropertyBlock.SetTexture(propertyName, texture2D);
                meshRenderer.SetPropertyBlock(materialPropertyBlock);

            }
            
            
        }

    }
    
    
}