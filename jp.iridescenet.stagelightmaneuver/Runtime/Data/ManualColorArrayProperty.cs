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
    public class ManualColorArrayProperty:SlmAdditionalArrayProperty
    {
        public SlmToggleValue<List<ColorPrimitiveValue>> colorValues;
        
        
        public ManualColorArrayProperty ()
        {
            propertyName = "Manual Color Array";
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = new List<ColorPrimitiveValue>() };
        }

        public override void ResyncArraySize(StageLightSupervisor stageLightSupervisor)
        {
            var colorPrimitiveValues = colorValues.value;
            if (colorPrimitiveValues.Count < stageLightSupervisor.AllStageLights.Count)
            {
                while (colorPrimitiveValues.Count < stageLightSupervisor.AllStageLights.Count)
                {
                    colorPrimitiveValues.Add(new ColorPrimitiveValue());
                }

            }

            if (colorPrimitiveValues.Count > stageLightSupervisor.AllStageLights.Count)
            {
                while (colorPrimitiveValues.Count > stageLightSupervisor.AllStageLights.Count)
                {
                    colorPrimitiveValues.RemoveAt(colorPrimitiveValues.Count - 1);
                }
            }

            for (int j = 0; j < stageLightSupervisor.AllStageLights.Count; j++)
            {
                // if not index is out of range
                if (j < colorPrimitiveValues.Count && j < stageLightSupervisor.AllStageLights.Count)
                {
                    if (colorPrimitiveValues[j] != null && stageLightSupervisor.AllStageLights[j] != null)
                    {
                        colorPrimitiveValues[j].name = stageLightSupervisor.AllStageLights[j].name;
                    }

                }

            }
        }


        public ManualColorArrayProperty (ManualColorArrayProperty other)
        {
            ColorPrimitiveValue[] copy = new ColorPrimitiveValue[other.colorValues.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.colorValues.value.CopyTo(copy);
            colorValues = new SlmToggleValue<List<ColorPrimitiveValue>>() { value = copy.ToList() };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            if (other is ManualColorArrayProperty)
            {
                ManualColorArrayProperty otherManualColorArrayProperty = (ManualColorArrayProperty) other;
                var copy = new ColorPrimitiveValue[otherManualColorArrayProperty.colorValues.value.Count];
                otherManualColorArrayProperty.colorValues.value.CopyTo(copy);
                if(otherManualColorArrayProperty.colorValues.propertyOverride)
                    colorValues.value = copy.ToList();
                
                if(otherManualColorArrayProperty.bpmOverrideData.propertyOverride)
                    bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(otherManualColorArrayProperty.bpmOverrideData);
            }              
        }
    }
    
}