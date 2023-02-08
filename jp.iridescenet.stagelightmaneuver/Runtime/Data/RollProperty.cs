using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    
   
    [Serializable]
    public class RollProperty:SlmAdditionalProperty
    {
        [DisplayName("Roll Transform")]public SlmToggleValue<MinMaxEasingValue> rollTransform;
       
        public RollProperty(RollProperty rollProperty)
        {
            propertyName = rollProperty.propertyName;

            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>()
            {
                propertyOverride = rollProperty.bpmOverrideData.propertyOverride,
                value = new BpmOverrideToggleValueBase(rollProperty.bpmOverrideData.value)
            };
            this.rollTransform = new SlmToggleValue<MinMaxEasingValue>()
            {
                propertyOverride =  rollProperty.rollTransform.propertyOverride,
                value =     new MinMaxEasingValue(rollProperty.rollTransform.value),
            };
            propertyOverride = rollProperty.propertyOverride;

        }

        public RollProperty()
        {
            propertyOverride = false;
            bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(){value = new BpmOverrideToggleValueBase()};
            rollTransform = new SlmToggleValue<MinMaxEasingValue>() {value = new MinMaxEasingValue()};
        }

        public override void ToggleOverride(bool toggle)
        {
            base.ToggleOverride(toggle);
            propertyOverride = toggle;
            rollTransform.propertyOverride = toggle;
            
        }
    }
    
    [Serializable]
    public class PanProperty:RollProperty
    {
        public PanProperty(PanProperty panProperty):base(panProperty)
        {
            propertyName = "Pan";
        }

        public PanProperty():base()
        {
            propertyName = "Pan";
        }
    }
    [Serializable]
    public class TiltProperty:RollProperty
    {
        public TiltProperty(TiltProperty tiltProperty):base(tiltProperty)
        {
            
            propertyName = "Tilt";
        }

        public TiltProperty():base()
        {
            
            propertyName = "Tilt";
        }
    }

}