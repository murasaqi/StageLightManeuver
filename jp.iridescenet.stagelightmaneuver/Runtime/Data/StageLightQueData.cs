using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [Serializable]
    public class StageLightQueData
    {

        [SerializeReference]public List<SlmProperty> stageLightProperties;
        public float weight = 1;
        
        
        public StageLightQueData(StageLightQueData stageLightQueData)
        {
            this.stageLightProperties = stageLightQueData.stageLightProperties;
            this.weight = stageLightQueData.weight;
        }
        
        public StageLightQueData()
        {
            stageLightProperties = new List<SlmProperty>();
            stageLightProperties.Add(new TimeProperty());
            weight = 1f;
        }
        public T TryGet<T>() where T : SlmProperty
        {
            foreach (var property in stageLightProperties)
            {
                if (property.GetType() == typeof(T))
                {
                    return property as T;
                }
            }
            return null;
        }

        public void TryAdd(Type T) 
        {
            if (T == typeof(LightFixture))
            {
                var find = stageLightProperties.Find(x => x.GetType() == typeof(LightProperty));
                if (find != null)
                {
                    // stageLightProperties.Add(lightProperty);
                    stageLightProperties.Add(new LightProperty());
                }
                
            }
            
            if (T == typeof(LightPanFixture))
            {
                var find = stageLightProperties.Find(x => x.GetType() == typeof(PanProperty));
                if (find != null)
                {
                    // stageLightProperties.Add(panProperty);
                    stageLightProperties.Add(new PanProperty());
                }
                
            }
            
            if (T == typeof(LightTiltFixture))
            {
                var find = stageLightProperties.Find(x => x.GetType() == typeof(TiltProperty));
                if (find != null)
                {
                    // stageLightProperties.Add(tiltProperty);
                    stageLightProperties.Add(new TiltProperty());
                }
                
            }
            
            if (T == typeof(GoboFixture))
            {
                var find = stageLightProperties.Find(x => x.GetType() == typeof(GoboProperty));
                if (find != null)
                {
                    // stageLightProperties.Add(goboProperty);
                    stageLightProperties.Add(new GoboProperty());
                }
            }
            
            if (T == typeof(DecalProperty))
            {
                var find = stageLightProperties.Find(x => x.GetType() == typeof(DecalProperty));
                if (find != null)
                {
                    // stageLightProperties.Add(decalProperty);
                    stageLightProperties.Add(new DecalProperty());
                }
            }
            
           
           
        }
    }
}