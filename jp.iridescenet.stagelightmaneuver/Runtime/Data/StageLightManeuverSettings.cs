using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace StageLightManeuver
{

    [CreateAssetMenu(fileName = "StageLightManeuverSetting", menuName = "StageLightManeuver/Settings")]
    public class StageLightManeuverSettings : ScriptableObject
    {
        // public static readonly string 
        public string exportProfilePath = "Assets/StageLightManeuver/Profiles/<Scene>/<ClipName>";

        public Dictionary<Type, int> SlmPropertyOrder;

        public void OnEnable()
        {
            SlmPropertyOrder = StageLightManeuverSettingsUtility.SlmPropertys
                .Select((x, i) => new { x, i })
                .ToDictionary(x => x.x, x => x.i - 2);
            SlmPropertyOrder[typeof(ClockProperty)] = -999;
            SlmPropertyOrder[typeof(StageLightOrderProperty)] = -998;
        }
    }
}
