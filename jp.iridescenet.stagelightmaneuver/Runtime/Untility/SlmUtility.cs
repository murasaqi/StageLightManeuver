using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

// using System.Reflection;

namespace StageLightManeuver
{
    public static class SlmUtility
    {


        public static List<Type> SlmAdditionalTypes = GetTypes(typeof(SlmAdditionalProperty));
        

        public static List<Type> GetTypes(Type T)
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies();

            var typeList = new List<Type>();
            foreach ( var assembly in assemblyList )
            {
                
                //
                if ( assembly == null )
                {
                    continue;
                }
                

                var types = assembly.GetTypes();
                typeList.AddRange(types.Where(t => t.IsSubclassOf(T))
                    .ToList());
              
            }

            return typeList;
        }
        
        public static Type GetTypeByClassName( string className )
        {
            foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
                foreach( Type type in assembly.GetTypes() ) {
                    if( type.Name == className ) {
                        return type;
                    }
                }
            }
            return null;
        }
        // public static float GetNormalizedTime(float time,float bpm, float bpmScale,float bpmOffset,int index,LoopType loopType)
        // {
        //     
        //     var scaledBpm = bpm * bpmScale;
        //     var duration = 60 / scaledBpm;
        //     var offset = duration* bpmOffset * index;
        //     var offsetTime = time + offset;
        //     var result = 0f;   
        //     var t = (float)offsetTime % duration;
        //     var normalisedTime = t / duration;
        //     
        //     if (loopType == LoopType.Loop)
        //     {
        //         result = normalisedTime;     
        //     }else if (loopType == LoopType.PingPong)
        //     {
        //         result = Mathf.PingPong(offsetTime / duration, 1f);
        //     }
        //     else if(loopType == LoopType.Fixed)
        //     {
        //         result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, normalisedTime);
        //     }
        //    
        //     return result;
        // }
    }
}