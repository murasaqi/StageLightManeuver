﻿using System;
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
    public class ManualLightArrayProperty:SlmAdditionalArrayProperty
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
            if (lightPrimitiveValue != null)
            {
                var newLightPrimitiveValue = new LightPrimitiveValue()
                {
                    name = lightPrimitiveValue.name,
                    intensity = lightPrimitiveValue.intensity,
                    angle = lightPrimitiveValue.angle,
                    innerAngle = lightPrimitiveValue.innerAngle,
                    range = lightPrimitiveValue.range
                };
                
                lightValues.value.Add(newLightPrimitiveValue);

            }
            else
            {
                var newLightPrimitiveValue = new LightPrimitiveValue()
                {
                    name = initialValue.name,
                    intensity = initialValue.intensity,
                    angle = initialValue.angle,
                    innerAngle = initialValue.innerAngle,
                    range = initialValue.range
                };
                
                lightValues.value.Add(newLightPrimitiveValue);
            }
            
        }

        public override void ResyncArraySize(StageLightSupervisor stageLightSupervisor)
        {
            
            var lightPrimitiveValues = lightValues.value;
            if (lightPrimitiveValues.Count < stageLightSupervisor.AllStageLights.Count)
            {
                while (lightPrimitiveValues.Count < stageLightSupervisor.AllStageLights.Count)
                {
                    AddLightPrimitive();
                }

            }

            if (lightPrimitiveValues.Count > stageLightSupervisor.AllStageLights.Count)
            {
                while (lightPrimitiveValues.Count > stageLightSupervisor.AllStageLights.Count)
                {
                    lightPrimitiveValues.RemoveAt(lightPrimitiveValues.Count - 1);
                }
            }

            for (int j = 0; j < stageLightSupervisor.AllStageLights.Count; j++)
            {
                // if not index is out of range
                if (j < lightPrimitiveValues.Count && j < stageLightSupervisor.AllStageLights.Count)
                {
                    if (lightPrimitiveValues[j] != null && stageLightSupervisor.AllStageLights[j] != null)
                    {
                        lightPrimitiveValues[j].name = stageLightSupervisor.AllStageLights[j].name;
                    }

                }

            }
            
        }


        public ManualLightArrayProperty (ManualLightArrayProperty other)
        {
            LightPrimitiveValue[] copy = new LightPrimitiveValue[other.lightValues.value.Count];
            propertyName = other.propertyName;
            propertyOverride = other.propertyOverride;
            other.lightValues.value.CopyTo(copy);
            lightValues = new SlmToggleValue<List<LightPrimitiveValue>>() { value = copy.ToList() };
        }

        public override void OverwriteProperty(SlmProperty other)
        {
            ManualLightArrayProperty manualLightArrayProperty = other as ManualLightArrayProperty;
            if (manualLightArrayProperty == null) return;
            var copy = new LightPrimitiveValue[manualLightArrayProperty.lightValues.value.Count];
            manualLightArrayProperty.lightValues.value.CopyTo(copy);
            if (manualLightArrayProperty.lightValues.propertyOverride)
                lightValues.value = copy.ToList();
        }
    }
    
}