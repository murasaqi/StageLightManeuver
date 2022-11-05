using System;
using System.Collections.Generic;

namespace StageLightManeuver
{
    
    [Serializable]
    public class LightArrayProperty:SlmAdditionalProperty
    {
        [DisplayName("Lights")] public SlmToggleValue<List<LightPrimitiveData>> lightToggleValues;

        public LightArrayProperty()
        {
            propertyName = "Light Array";
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = false,
                value = new BpmOverrideToggleValueBase()
            };
            
            
            lightToggleValues = new SlmToggleValue<List<LightPrimitiveData>>()
            {
                propertyOverride = false,
                value = new List<LightPrimitiveData>()
            };
        }
        
        public LightArrayProperty(LightArrayProperty other)
        {
            propertyName = other.propertyName;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(other.bpmOverrideData);
            var copy = new List<LightPrimitiveData>();
            foreach (var lightPrimitiveProperty in other.lightToggleValues.value)
            {
                copy.Add(new LightPrimitiveData(lightPrimitiveProperty));
            }
            lightToggleValues = new SlmToggleValue<List<LightPrimitiveData>>()
            {
                propertyOverride = other.lightToggleValues.propertyOverride,
                value =copy
            };
        }
    }
}