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
        
        public static Color GetHDRColor(Color color, float intensity)
        {
            return new Color(color.r, color.g, color.b, color.a) *Mathf.Pow(2.0f,intensity);
        }
        public static Gradient CloneGradient(Gradient gradient)
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
            return newGradient;
        }
    }
}