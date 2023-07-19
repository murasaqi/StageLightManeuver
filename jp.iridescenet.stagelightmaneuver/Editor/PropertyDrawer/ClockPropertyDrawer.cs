using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    [CustomPropertyDrawer(typeof(ClockProperty))]
    public class ClockPropertyDrawer : SlmPropertyDrawer
    {
        public ClockPropertyDrawer()
        {
            base._marginBottom = 0;
        }
    }
}
