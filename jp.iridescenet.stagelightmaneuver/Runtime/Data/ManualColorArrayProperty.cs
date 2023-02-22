using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{

    [Serializable]
    public class ColorPrimitiveValue
    {
        public string name;
        public Color color;

        public ColorPrimitiveValue()
        {
            color = Color.white;
            name = "Color";
        }
    }
    
    
    
    [Serializable]
    public class ManualColorArrayProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<List<ColorPrimitiveValue>> colorValues;
        
        
        public ManualColorArrayProperty ()
        {
            propertyName = "Manual Color Array";
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = new List<ColorPrimitiveValue>() };
        }
        
        
        public ManualColorArrayProperty (ManualColorArrayProperty other)
        {
            ColorPrimitiveValue[] copy = new ColorPrimitiveValue[other.colorValues.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.colorValues.value.CopyTo(copy);
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = copy.ToList() };
        }
    }
    
}