using System;
using UnityEngine;

namespace StageLightManeuver
{

    [Serializable]
    [AddComponentMenu("")]
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