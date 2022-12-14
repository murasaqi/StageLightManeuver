using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace StageLightManeuver
{

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DisplayNameAttribute : System.Attribute
    {
        public string name;
        public DisplayNameAttribute(string name)
        {
            this.name = name;
        }
    }
    
    
    [Serializable]
    public class SlmToggleValueBase
    {
        [SerializeField] public bool propertyOverride = false;
    }

    [Serializable]
    public class SlmToggleValue<T>:SlmToggleValueBase
    {
        public T value;
        
        public SlmToggleValue(SlmToggleValue<T> slmToggleValue)
        {
            propertyOverride = slmToggleValue.propertyOverride;
            this.value = slmToggleValue.value;
        }

        public SlmToggleValue()
        {
            propertyOverride = false;
            value = default;
        }
    }


    [Serializable]
    public class SlmProperty:SlmToggleValueBase
    {
        public string propertyName;
        
        public virtual void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
        }
        
    }
    
    
    [Serializable]
    public class BpmOverrideToggleValueBase:SlmToggleValueBase
    {
        [DisplayName("Loop Type")] public LoopType loopType = LoopType.Loop;
        [DisplayName("Override Time")] public bool bpmOverride = false;
        [DisplayName("Offset Time")] public float offsetTime = 0;
        [DisplayName("BPM Scale")] public float bpmScale = 1;
        [DisplayName("Child Stagger")] public float childStagger = 0;
       
        public BpmOverrideToggleValueBase()
        {
            propertyOverride = false;
            bpmScale = 1;
            childStagger = 0;
            loopType = LoopType.Loop;
            bpmOverride = false;
            offsetTime = 0;
        }
        
        public BpmOverrideToggleValueBase(BpmOverrideToggleValueBase bpmOverrideToggleValueBase)
        {
            propertyOverride = bpmOverrideToggleValueBase.propertyOverride;
            bpmScale = bpmOverrideToggleValueBase.bpmScale;
            childStagger = bpmOverrideToggleValueBase.childStagger;
            loopType = bpmOverrideToggleValueBase.loopType;
            bpmOverride = bpmOverrideToggleValueBase.bpmOverride;
            offsetTime = bpmOverrideToggleValueBase.offsetTime;
        }
    }
    
    
    [Serializable]
    public class SlmAdditionalProperty:SlmProperty
    {
        [DisplayName("BPM Override")]public SlmToggleValue<BpmOverrideToggleValueBase> bpmOverrideData = new SlmToggleValue<BpmOverrideToggleValueBase>(); 
        
    }

    
    
    [Serializable]
    public class ClipProperty
    {
        public float clipStartTime;
        public float clipEndTime;
        
        public ClipProperty()
        {
            clipStartTime = 0f;
            clipEndTime = 0f;
        }

        public ClipProperty(ClipProperty other)
        {
            clipStartTime = other.clipStartTime;
            clipEndTime = other.clipEndTime;
        }
    }







}