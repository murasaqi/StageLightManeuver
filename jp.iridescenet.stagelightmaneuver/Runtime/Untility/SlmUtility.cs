using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;

// using System.Reflection;

namespace StageLightManeuver
{
    
    public static class SlmUtility
    {


        public static string BaseExportPath = "Assets/StageLightManeuver/Profiles/<Scene>/<ClipName>";

        public static string GetExportPath(string path, string clipName)
        {
            return path.Replace("<Scene>", SceneManager.GetActiveScene().name).Replace("<ClipName>", clipName);
        }

        public static List<Type> SlmAdditionalTypes = GetTypes(typeof(SlmAdditionalProperty));
        
        
          public static float GetNormalizedTime(float time ,StageLightQueData queData, Type propertyType,int index = 0)
        {
            var additionalProperty = queData.TryGetAdditionalProperty(propertyType) as SlmAdditionalProperty;
            var timeProperty = queData.TryGet<TimeProperty>();
            var weight = queData.weight;
            if (additionalProperty == null || timeProperty == null) return 0f;
            var bpm = timeProperty.bpm.value;
            var bpmOffset = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.childStagger : timeProperty.childStagger.value;
            var bpmScale = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.bpmScale : timeProperty.bpmScale.value;
            var loopType = additionalProperty.bpmOverrideData.value.bpmOverride ? additionalProperty.bpmOverrideData.value.loopType : timeProperty.loopType.value;
            var offsetTime = additionalProperty.bpmOverrideData.value.bpmOverride
                ? additionalProperty.bpmOverrideData.value.offsetTime
                : timeProperty.offsetTime.value;
            var clipProperty = timeProperty.clipProperty;
            var t = GetNormalizedTime(time+offsetTime,bpm,bpmOffset,bpmScale,clipProperty,loopType,index);
            return t;
        }
        
        public static float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType, int index = 0)
        {
            
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * (index+1);
            var offsetTime = time + offset;
             var result = 0f;
            var t = (float)offsetTime % duration;
            var normalisedTime = t / duration;
            
            if (loopType == LoopType.Loop)
            {
                result = normalisedTime;     
            }else if (loopType == LoopType.PingPong)
            {
                result = Mathf.PingPong(offsetTime / duration, 1f);
            }
            else if(loopType == LoopType.Fixed)
            {
                result = Mathf.InverseLerp(clipProperty.clipStartTime, clipProperty.clipEndTime, time);
            }
           
            return result;
        }

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
        
        public static Color GetHDRColor(Color color, float intensity)
        {
            return new Color(color.r, color.g, color.b, color.a) *Mathf.Pow(2.0f,intensity);
        }
        
        public static AnimationCurve CopyAnimationCurve(AnimationCurve curve)
        {
            var newCurve = new AnimationCurve();
            var copyKeys = new Keyframe[curve.keys.Length];
            curve.keys.CopyTo(copyKeys, 0);
            newCurve.keys = copyKeys;
            newCurve.preWrapMode = curve.preWrapMode;
            newCurve.postWrapMode = curve.postWrapMode;
            return newCurve;
        }
        public static Gradient CopyGradient(Gradient gradient)
        {
            Gradient newGradient = new Gradient();
            var colorKeys = new GradientColorKey[gradient.colorKeys.Length];
            var alphaKeys = new GradientAlphaKey[gradient.alphaKeys.Length];
            
            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                colorKeys[i] = gradient.colorKeys[i];
            }
            
            for (int i = 0; i < gradient.alphaKeys.Length; i++)
            {
                alphaKeys[i] = gradient.alphaKeys[i];
            }
            newGradient.SetKeys(colorKeys, alphaKeys);
            newGradient.mode = gradient.mode;
            
            return newGradient;
        }
    }
}