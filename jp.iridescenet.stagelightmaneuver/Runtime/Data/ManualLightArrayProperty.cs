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
    }
    
    
    [Serializable]
    public class ManualLightArrayProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<List<LightPrimitiveValue>> lightValues;
        
        
        public ManualLightArrayProperty ()
        {
            propertyName = "Manual Light Array";
            lightValues = new SlmToggleValue<List<LightPrimitiveValue>>() { value = new List<LightPrimitiveValue>() };
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