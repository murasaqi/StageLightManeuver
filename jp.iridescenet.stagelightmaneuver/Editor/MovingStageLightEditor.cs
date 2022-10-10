using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageLightManeuver
{
    [CustomEditor(typeof(MovingStageLight))]
    [CanEditMultipleObjects]
    public class MovingStageLightEditor:Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            // return base.CreateInspectorGUI();
            
            var movingStageLight = target as MovingStageLight;
            var indexField = new PropertyField(serializedObject.FindProperty("index"));
            indexField.SetEnabled(false); 
            root.Add(indexField);
            root.Add(new PropertyField(serializedObject.FindProperty("stageLightFixtures")));
           
            

            var fixtureList = new List<string>();
            
            
            fixtureList.Add("Add New Fixture");
            var executingAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = executingAssembly.GetReferencedAssemblies();

            foreach ( var assemblyName in referencedAssemblies )
            {
                var assembly = Assembly.Load( assemblyName );

                if ( assembly == null )
                {
                    continue;
                }

                var types = assembly.GetTypes();

                types.Where(t => t.IsSubclassOf(typeof(StageLightFixtureBase)))
                .ToList()
                .ForEach(t =>
                {
                    
                    // Debug.Log(assembly.FullName);
                    // Debug.Log(t.FullName);
                    if (movingStageLight.StageLightFixtures.Find(x => x.GetType().Name == t.Name) == null)
                    {   
                        fixtureList.Add(t.Name);   
                    }
                });
            }

            var center = new VisualElement();
            center.style.alignItems = Align.Center;
            var popupField = new PopupField<string>(fixtureList, 0);
            popupField.SetEnabled( fixtureList.Count > 1 );
            popupField.RegisterValueChangedCallback((evt =>
            {
                if (popupField.index != 0)
                {
                    var type = GetTypeByClassName(popupField.value);
                    Debug.Log(type);
                    MethodInfo mi = typeof(GameObject).GetMethod(
                        "AddComponent",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new Type[0],
                        null
                    );
                    MethodInfo bound = mi.MakeGenericMethod(type);
                    
                    var fixture =bound.Invoke(movingStageLight.gameObject, null);

                    // var fixture = Convert.ChangeType(fixture, type);
                    movingStageLight.FindFixtures();
                }
            }));
            center.Add(popupField);
            root.Add(center);
            root.Add(new PropertyField(serializedObject.FindProperty("stageLightChild")));

            return root;
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
    }
}