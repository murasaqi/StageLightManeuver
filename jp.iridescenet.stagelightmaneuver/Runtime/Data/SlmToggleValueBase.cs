using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


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
        public int propertyOrder = 0;
        public virtual void ToggleOverride(bool toggle)
        {
            propertyOverride = toggle;
        }
        
        public virtual void OverwriteProperty(SlmProperty other)
        {
        }

    }
    
    
    [Serializable]
    public class ClockOverrideToggleValueBase:SlmToggleValueBase
    {
        [DisplayName("Loop Type")] public LoopType loopType = LoopType.Loop;
        // [DisplayName("Override Time")] public bool bpmOverride = false;
        [DisplayName("Offset Time")] public float offsetTime = 0;
        [DisplayName("BPM Scale")] public float bpmScale = 1;
        [DisplayName("Child Stagger")] public float childStagger = 0;
       
        public ClockOverrideToggleValueBase()
        {
            propertyOverride = false;
            bpmScale = 1;
            childStagger = 0;
            loopType = LoopType.Loop;
            // bpmOverride = false;
            offsetTime = 0;
        }
        
        public ClockOverrideToggleValueBase(ClockOverrideToggleValueBase clockOverrideToggleValueBase)
        {
            propertyOverride = clockOverrideToggleValueBase.propertyOverride;
            bpmScale = clockOverrideToggleValueBase.bpmScale;
            childStagger = clockOverrideToggleValueBase.childStagger;
            loopType = clockOverrideToggleValueBase.loopType;
            // bpmOverride = bpmOverrideToggleValueBase.bpmOverride;
            offsetTime = clockOverrideToggleValueBase.offsetTime;
        }
    }
    
    
    [Serializable]
    public class SlmAdditionalProperty:SlmProperty
    {
        [FormerlySerializedAs("bpmOverride")] [FormerlySerializedAs("bpmOverrideData")] [DisplayName("BPM Override")]public SlmToggleValue<ClockOverrideToggleValueBase> clockOverride = new SlmToggleValue<ClockOverrideToggleValueBase>();
    }
    
    public class SlmAdditionalArrayProperty:SlmProperty
    {
        [DisplayName("BPM Override")]public SlmToggleValue<ClockOverrideToggleValueBase> clockOverride = new SlmToggleValue<ClockOverrideToggleValueBase>();
        public virtual void ResyncArraySize(StageLightSupervisor stageLightSupervisor)
        {
            
        }
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