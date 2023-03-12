// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace StageLightManeuver
// {
//     [Serializable]
//     public class StageLightQueData
//     {
//
//         [SerializeReference]public List<SlmProperty> stageLightProperties = new List<SlmProperty>();
//         public float weight = 1;
//         
//         
//         public StageLightQueData(StageLightQueData stageLightQueData)
//         {
//             this.stageLightProperties = stageLightQueData.stageLightProperties;
//             this.weight = stageLightQueData.weight;
//         }
//         
//         public StageLightQueData()
//         {
//             stageLightProperties = new List<SlmProperty>();
//             stageLightProperties.Clear();
//             // stageLightProperties.Add(new TimeProperty());
//             weight = 1f;
//         }
//         public T TryGet<T>() where T : SlmProperty
//         {
//             foreach (var property in stageLightProperties)
//             {
//                 if (property == null)
//                 {
//                     continue;
//                 }
//                 if (property.GetType() == typeof(T))
//                 {
//                     return property as T;
//                 }
//             }
//             return null;
//         }
//         
//         public SlmAdditionalProperty TryGetAdditionalProperty(Type T) 
//         {
//             foreach (var property in stageLightProperties)
//             {
//                 if(property == null)
//                 {
//                     continue;
//                 }
//                 if (property.GetType() ==T)
//                 {
//                     return property as SlmAdditionalProperty;
//                 }
//             }
//             return null;
//         }
//         
//         
//         
//
//         public SlmAdditionalProperty TryAdd(Type T) 
//         {
//             
//             foreach (var property in stageLightProperties)
//             {
//                 if (property == null)
//                 {
//                     continue;
//                 }
//                 if (property.GetType() == T)
//                 {
//                     return property as SlmAdditionalProperty;
//                 }
//             }
//             
//             
//             var instance =  Activator.CreateInstance(T, new object[] { }) as SlmAdditionalProperty;
//             stageLightProperties.Add(instance);
//
//             return instance;
//             // if (T == typeof(LightFixture))
//             // {
//             //     var find = stageLightProperties.Find(x => x.GetType() == typeof(LightProperty));
//             //     if (find != null)
//             //     {
//             //         // stageLightProperties.Add(lightProperty);
//             //         var lightProperty = find as LightProperty;
//             //         stageLightProperties.Add(lightProperty);
//             //         return lightProperty;
//             //     }
//             //     
//             // }
//             //
//             // if (T == typeof(LightPanFixture))
//             // {
//             //     var find = stageLightProperties.Find(x => x.GetType() == typeof(PanProperty));
//             //     if (find != null)
//             //     {
//             //         // stageLightProperties.Add(panProperty);
//             //         var panProperty = find as PanProperty;
//             //         stageLightProperties.Add(new PanProperty());
//             //     }
//             //     
//             // }
//             //
//             // if (T == typeof(LightTiltFixture))
//             // {
//             //     var find = stageLightProperties.Find(x => x.GetType() == typeof(TiltProperty));
//             //     if (find != null)
//             //     {
//             //         // stageLightProperties.Add(tiltProperty);
//             //         stageLightProperties.Add(new TiltProperty());
//             //     }
//             //     
//             // }
//             //
//             // if (T == typeof(GoboFixture))
//             // {
//             //     var find = stageLightProperties.Find(x => x.GetType() == typeof(GoboProperty));
//             //     if (find != null)
//             //     {
//             //         // stageLightProperties.Add(goboProperty);
//             //         stageLightProperties.Add(new GoboProperty());
//             //     }
//             // }
//             //
//             // if (T == typeof(DecalProperty))
//             // {
//             //     var find = stageLightProperties.Find(x => x.GetType() == typeof(DecalProperty));
//             //     if (find != null)
//             //     {
//             //         // stageLightProperties.Add(decalProperty);
//             //         stageLightProperties.Add(new DecalProperty());
//             //     }
//             // }
//
//
//
//         }
//     }
// }