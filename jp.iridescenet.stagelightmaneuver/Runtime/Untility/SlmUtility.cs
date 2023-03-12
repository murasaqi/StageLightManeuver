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

        public static List<Type> SlmPropertyTypes = GetTypes(typeof(SlmProperty));
        
        public static float GetOffsetTime(StageLightQueueData queData, Type propertyType,int index)
        {
            var additionalProperty = queData.TryGet(propertyType) as SlmAdditionalProperty;
            var clockProperty = queData.TryGet<ClockProperty>();
            var bpm =clockProperty.bpm.value;
            var bpmOffset = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.childStagger : clockProperty.staggerDelay.value;
            var bpmScale = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : clockProperty.bpmScale.value;
            var scaledBpm = bpm * bpmScale;
            var duration = 60 / scaledBpm;
            var offset = duration* bpmOffset * (index+1);
            return offset;
        }
        
        public static float GetNormalizedTime(float currentTime, ClockProperty clockOverride, SlmAdditionalProperty slmAdditionalProperty)
        {
            
            var bpmOverrideData = slmAdditionalProperty.clockOverride;
            var offsetTime = bpmOverrideData.propertyOverride ? bpmOverrideData.value.offsetTime : clockOverride.offsetTime.value;
            var bpm =  clockOverride.bpm.value;
            var bpmOffset =bpmOverrideData.propertyOverride ? bpmOverrideData.value.childStagger : clockOverride.staggerDelay.value;
            var bpmScale = bpmOverrideData.propertyOverride ? bpmOverrideData.value.bpmScale : clockOverride.bpmScale.value;
            var loopType = bpmOverrideData.propertyOverride ? bpmOverrideData.value.loopType : clockOverride.loopType.value;
            
            var arrayStaggerValue = bpmOverrideData.propertyOverride ? bpmOverrideData.value.arrayStaggerValue: clockOverride.arrayStaggerValue;
            return SlmUtility.GetNormalizedTime(currentTime+offsetTime, bpm, bpmOffset,bpmScale,clockOverride.clipProperty, loopType,arrayStaggerValue);
        }

        
        public static float GetNormalizedTime(float time ,StageLightQueueData queData, Type propertyType,int index = 0)
        {
            var additionalProperty = queData.TryGet(propertyType);
            var clockProperty = queData.TryGet<ClockProperty>();
            var weight = queData.weight;
            if (additionalProperty == null || clockProperty == null) return 0f;
            var bpm = clockProperty.bpm.value;
            var stagger = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.childStagger : clockProperty.staggerDelay.value;
            var bpmScale = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.bpmScale : clockProperty.bpmScale.value;
            var loopType = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.loopType : clockProperty.loopType.value;
            var offsetTime = additionalProperty.clockOverride.propertyOverride
              ? additionalProperty.clockOverride.value.offsetTime
              : clockProperty.offsetTime.value;
            var clipProperty = clockProperty.clipProperty;
            var arrayStaggerValue = additionalProperty.clockOverride.propertyOverride ? additionalProperty.clockOverride.value.arrayStaggerValue : clockProperty.arrayStaggerValue;
            var t = GetNormalizedTime(time+offsetTime,bpm,stagger,bpmScale,clipProperty,loopType,arrayStaggerValue,index);
            return t; 
        }
          
        public static List<StageLightProfile> GetProfileInProject()
        {
              // Debug.Log(Application.dataPath);
              // Get file in Unity project folderselectedProfileIndex
            

              // var guids = AssetDatabase.FindAssets("t:StageLightProfile a:all");
              var guids = AssetDatabase.FindAssets("t:StageLightProfile");
              var profiles = new List<StageLightProfile>();
              foreach (var guid in guids)
              {
                  var path = AssetDatabase.GUIDToAssetPath(guid);
                  var profile = AssetDatabase.LoadAssetAtPath<StageLightProfile>(path);
                  profiles.Add(profile);
              }
            return profiles;
        }
        
        public static float GetNormalizedTime(float time,float bpm, float bpmOffset,float bpmScale,ClipProperty clipProperty,LoopType loopType, ArrayStaggerValue arrayStaggerValue, int index = 0)
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
            else if(loopType == LoopType.FixedStagger)
            {
                // var staggerStartEnd =arrayStaggerValue.GetStaggerStartEnd(index);
                var clipDuration = clipProperty.clipEndTime - clipProperty.clipStartTime;
                result = arrayStaggerValue.Evaluate(time, clipDuration, index);
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