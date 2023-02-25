using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{


    [Serializable]
    public class LightPrimitiveValue
    {
        public string name;
        public float intensity;
        public float angle;
        public float innerAngle;
        public float range;
        
        public LightPrimitiveValue()
        {
            name = "Light";
            intensity = 1f;
            angle = 30f;
            innerAngle = 0f;
            range = 10f;
        }
        
        public LightPrimitiveValue(LightPrimitiveValue other)
        {
            name = other.name;
            intensity = other.intensity;
            angle = other.angle;
            innerAngle = other.innerAngle;
            range = other.range;
        }
    }
    
    
    [Serializable]
    public class ManualLightArrayProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<List<LightPrimitiveValue>> lightValues;
        
        public LightPrimitiveValue initialValue = new LightPrimitiveValue()
        {
            name = "Light",
            intensity = 1f,
            angle = 30f,
            innerAngle = 0f,
            range = 10f
        };
        
        public ManualLightArrayProperty ()
        {
            propertyName = "Manual Light Array";
            lightValues = new SlmToggleValue<List<LightPrimitiveValue>>() { value = new List<LightPrimitiveValue>() };
        }
        
        
        public void AddLightPrimitive(LightPrimitiveValue lightPrimitiveValue = null)
        {
            var lightPrimitive = lightPrimitiveValue ?? initialValue;
            lightValues.value.Add(lightPrimitive);
        }
        
        
        public ManualLightArrayProperty (ManualLightArrayProperty other)
        {
            LightPrimitiveValue[] copy = new LightPrimitiveValue[other.lightValues.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.lightValues.value.CopyTo(copy);
            lightValues = new SlmToggleValue<List<LightPrimitiveValue>>() { value = copy.ToList() };
        }
    }
    
}