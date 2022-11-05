using System;
using UnityEngine;

namespace StageLightManeuver
{

    [Serializable]
    public class LightPrimitiveProperty
    {
        public float intensity;
        public Gradient gradient;
        public float outerSpotAngle;
        public float innerSpotAngle;
        public float range;
    }
    public class ManualLightArrayFixture:StageLightFixtureBase
    {
        
    }
}