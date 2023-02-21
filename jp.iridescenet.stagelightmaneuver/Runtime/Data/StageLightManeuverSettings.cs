using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{

    [CreateAssetMenu(fileName = "StageLightManeuverSetting", menuName = "StageLightManeuver/Settings")]
    public class StageLightManeuverSettings : ScriptableObject
    {
        public string exportProfilePath = "Assets/StageLightManeuver/Profiles/<Scene>/<ClipName>";

    }
}
