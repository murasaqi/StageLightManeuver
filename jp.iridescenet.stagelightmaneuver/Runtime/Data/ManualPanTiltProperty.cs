using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{


    [Serializable]
    public class PanTiltPrimitive
    {
        public string name;
        public float pan = 0f;
        public float tilt = 0f;
    }


    [Serializable]
    // ManualPantiltMode enum Overwrite, Add, Subtract, Multiply, Divide
    public enum ManualPanTiltMode
    {
        Overwrite,
        Add,
    }
    
    
    [Serializable]
    public class ManualPanTiltProperty:SlmAdditionalProperty
    {
        public SlmToggleValue<ManualPanTiltMode> mode;
        public SlmToggleValue<List<PanTiltPrimitive>> positions;
        
        public ManualPanTiltProperty ()
        {
            propertyName = "Manual Pan Tilt";
            positions = new SlmToggleValue<List<PanTiltPrimitive>>() { value = new List<PanTiltPrimitive>() };
            mode = new SlmToggleValue<ManualPanTiltMode>() { value = ManualPanTiltMode.Overwrite };
        }
        
        
        public ManualPanTiltProperty (ManualPanTiltProperty other)
        {
            PanTiltPrimitive[] copy = new PanTiltPrimitive[other.positions.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.positions.value.CopyTo(copy);
            positions = new SlmToggleValue<List<PanTiltPrimitive>>() { value = copy.ToList() };
            mode = new SlmToggleValue<ManualPanTiltMode>() { value = other.mode.value };
        }
    }
    
}