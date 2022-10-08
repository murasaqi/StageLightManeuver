using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace StageLightManeuver
{
    [Serializable]
    public struct SerializedPropertyData
    {
        public string type;
        public string jsonInfo;
    }
    [CreateAssetMenu(fileName = "New StageLightSetting", menuName = "ScriptableObject/StageLightSetting")]
    public class StageLightProfile: ScriptableObject
    {
        // public TimeProperty timeProperty;
        // public LightProperty lightProperty;
        // public PanProperty panProperty;
        // public TiltProperty tiltProperty;
        // public DecalProperty decalProperty;
        // public GoboProperty goboProperty;
        public bool isUpdateGuiFlag = false;
        [SerializeReference]public List<SlmProperty> stageLightProperties = new List<SlmProperty>();
  
        
        
        [ContextMenu("Init")]
        public void Init()
        {
            // timeProperty = new TimeProperty();
            // lightProperty = new LightProperty();
            // panProperty = new PanProperty();
            // tiltProperty = new TiltProperty();
            // decalProperty = new DecalProperty();
            // goboProperty = new GoboProperty();
            // InitializePropertyList();
        }
        
        // public void InitializePropertyList()
        // {
            // stageLightProperties.Clear();
            // stageLightProperties.Add(timeProperty);
            // stageLightProperties.Add(lightProperty);
            // stageLightProperties.Add(panProperty);
            // stageLightProperties.Add(tiltProperty);
            // stageLightProperties.Add(decalProperty);
            // stageLightProperties.Add(goboProperty);
        // }
        
        
        // public void ApplyListToProperties()
        // {
        //     var _stageLightBaseProperty =
        //         (TimeProperty)stageLightProperties.Find(x => x.GetType() == typeof(TimeProperty));
        //     if(_stageLightBaseProperty != null)
        //         timeProperty = _stageLightBaseProperty;
        //     
        //     var _lightProperty =    
        //         (LightProperty)stageLightProperties.Find(x => x.GetType() == typeof(LightProperty));
        //     if(_lightProperty != null)
        //         lightProperty = _lightProperty;
        //     
        //     var _panProperty =  
        //         (PanProperty)stageLightProperties.Find(x => x.GetType() == typeof(PanProperty));
        //     if(_panProperty != null)
        //         panProperty = _panProperty;
        //     
        //     var _tiltProperty =
        //         (TiltProperty)stageLightProperties.Find(x => x.GetType() == typeof(TiltProperty));
        //     if(_tiltProperty != null)
        //         tiltProperty = _tiltProperty;
        //     
        //     var _decalProperty =    
        //         (DecalProperty)stageLightProperties.Find(x => x.GetType() == typeof(DecalProperty));
        //     if(_decalProperty != null)  
        //         decalProperty = _decalProperty;
        //     
        //     var _goboProperty =
        //         (GoboProperty)stageLightProperties.Find(x => x.GetType() == typeof(GoboProperty));
        //     if(_goboProperty != null)
        //         goboProperty = _goboProperty;
        //     
        // }

        public void TryCreatePropertyListByComponentList(List<StageLightFixtureBase> stageLightExtensions)
        {
            stageLightProperties.Clear();
            foreach (var extension in stageLightExtensions)
            {
                Debug.Log(extension);
                if (extension.GetType() == typeof(LightFixture))
                {
                    stageLightProperties.Add(new LightProperty());
                }
            
                if (extension.GetType() == typeof(LightPanFixture))
                {
                    stageLightProperties.Add(new PanProperty());
                }
            
                if (extension.GetType() == typeof(LightTiltFixture))
                {
                    stageLightProperties.Add(new TiltProperty());
                }
                
                if (extension.GetType() == typeof(DecalFixture))
                {
                    stageLightProperties.Add(new DecalProperty());
                }
                
                if (extension.GetType() == typeof(GoboFixture))
                {
                    stageLightProperties.Add(new GoboProperty());
                }
            }
         
        }

        public T TryGet<T>() where T : SlmProperty
        {
            var result = stageLightProperties.Find(x => x.GetType() == typeof(T));
            return result as T;
        }
        public StageLightProfile()
        {
            Init();
        }
        public StageLightProfile Clone()
        {
            var result = CreateInstance<StageLightProfile>();
            // result.stageLightProperties.Clear();

            foreach (var stageLightProperty in stageLightProperties)
            {
                var type = stageLightProperty.GetType();
                result.stageLightProperties.Add(Activator.CreateInstance(type, BindingFlags.CreateInstance, null, new object[]{stageLightProperty}, null)
                    as SlmProperty);
            }
           
           
            return result;
        }
        //
        // [ContextMenu("Serialize")]
        // public void Serialize()
        // {
        //     serializedData.Clear();
        //     foreach (var stageLightProperty in stageLightProperties)
        //     {
        //         serializedData.Add(new SerializedPropertyData()
        //         {
        //             type = stageLightProperty.GetType().ToString(),
        //             jsonInfo = JsonUtility.ToJson(Convert.ChangeType(stageLightProperty, stageLightProperty.GetType()))
        //         });
        //     }
        // }
        //
        // [ContextMenu("Deserialize")]
        // public void Desirialize()
        // {
        //     stageLightProperties.Clear();
        //     foreach (var serializedDatum in serializedData)
        //     {
        //         var type = Type.GetType(serializedDatum.type);
        //         var result = Activator.CreateInstance(type, BindingFlags.CreateInstance, null, new object[]{}, null) as StageLightProperty;
        //         JsonUtility.FromJsonOverwrite(serializedDatum.jsonInfo, result);
        //         stageLightProperties.Add(result);
        //     }
        // }
        
        // public StageLightProfile(StageLightProfile profile)
        // {
        //     stageLightProperties = new List<StageLightProperty>();
        //     foreach (var stageLightProperty in profile.stageLightProperties)
        //     {
        //         stageLightProperties.Add(stageLightProperty.Clone());
        //     }
        // }
        // public Dictionary<Type,StageLightData> stageLightDataDictionary = new Dictionary<Type, StageLightData>();
        
        // public Dictionary<Type,StageLightData> GetStageLightDataDictionary()
        // {
        //     Dictionary<Type, StageLightData> stageLightDataDictionary = new Dictionary<Type, StageLightData>();
        //     foreach (var data in stageLightData)
        //     {
        //         stageLightDataDictionary.Add(data.GetType(),data);
        //     }
        //
        //     return stageLightDataDictionary;
        // }

    }
}
