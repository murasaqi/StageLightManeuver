using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageLightManeuver
{
    [CreateAssetMenu(fileName = "StageLightManeuverSetting", menuName = "StageLightManeuver/Settings")]
    public class StageLightManeuverSettings : ScriptableObject
    {
        public string exportProfilePath = SlmSettingsUtility.BaseExportProfilePath;
        public Dictionary<Type, int> SlmPropertyOrder;
        // public GUIStyle[] SlmPropertyStyles;

        public void OnEnable()
        {
            SlmPropertyOrder = SlmSettingsUtility.SlmPropertys
                .Select((x, i) => new { x, i })
                .ToDictionary(x => x.x, x => x.i - 2);
            SlmPropertyOrder[typeof(ClockProperty)] = -999;
            SlmPropertyOrder[typeof(StageLightOrderProperty)] = -998;
        }
    }
}
