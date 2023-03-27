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
        public int sortOrder = 0;
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

        public virtual void OnProcessFrame(float time, float clipStart, float clipDuration)
        {
            
        }
        
        public virtual void OverwriteProperty(SlmProperty other)
        {
        }

    }
    
    
    [Serializable]
      public class ClockOverride
    {
        [DisplayName("Loop Type")] public LoopType loopType = LoopType.Loop;
        [DisplayName("Offset Time")] public float offsetTime;
        [DisplayName("BPM Scale")]public float bpmScale;
        [DisplayName("Child Stagger")]public float childStagger;
        public ArrayStaggerValue arrayStaggerValue = new ArrayStaggerValue();

        public ClockOverride()
        {
            loopType = LoopType.Loop;
            bpmScale = 1f;
            offsetTime = 0f;
            childStagger = 0f;
            arrayStaggerValue = new ArrayStaggerValue();
        }
        
        public ClockOverride(ClockOverride clockOverride)
        {
            loopType = clockOverride.loopType;
            bpmScale = clockOverride.bpmScale;
            offsetTime = clockOverride.offsetTime;
            childStagger = clockOverride.childStagger;
            arrayStaggerValue = new ArrayStaggerValue(clockOverride.arrayStaggerValue);
        }
        
       
        
        
    }
      
      public interface IArrayProperty
      {
          void ResyncArraySize(StageLightSupervisor stageLightSupervisor);
      } 
    
    [Serializable]
    public class SlmAdditionalProperty:SlmProperty,IArrayProperty
    {
        public SlmToggleValue<ClockOverride> clockOverride = new  SlmToggleValue<ClockOverride>()
        {
            value = new ClockOverride(),
            sortOrder = -999
        };

        public virtual void ResyncArraySize(StageLightSupervisor stageLightSupervisor)
        {
            if(clockOverride.value != null && clockOverride.value.arrayStaggerValue != null)
                clockOverride.value.arrayStaggerValue.ResyncArraySize(stageLightSupervisor);
        }
    }
    
 
    
    // [Serializable]
    // public class SlmArrayProperty:SlmProperty,IArrayProperty
    // {
    //     public SlmToggleValue<ClockOverride> clockOverride = new  SlmToggleValue<ClockOverride>()
    //     {
    //         sortOrder = -999
    //     };
    //     public virtual void ResyncArraySize(StageLightSupervisor stageLightSupervisor)
    //     {
    //         
    //     }
    // }
    //
   
    
    
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