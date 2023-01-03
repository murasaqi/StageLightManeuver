using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace StageLightManeuver
{
    [AddComponentMenu("")]
    public class ManualPanTiltFixture : StageLightFixtureBase
    {
        public List<Vector2> panTiltPositions = new List<Vector2>();
        [SerializeReference]public List<IStageLightFixture> stageLights = new List<IStageLightFixture>();


        public override void Init()
        {
            base.Init();
            updateOrder = 999;
        }

        public override void EvaluateQue(float time)
        {
            for (int i = 0; i < panTiltPositions.Count; i++)
            {
                if (i < stageLights.Count)
                {
                    var iStageLightFixture = stageLights[i];
                    var panFixture = iStageLightFixture.TryGetFixture<LightPanFixture>();
                    var rotationVector = panFixture.LightTransformType == LightTransformType.Pan ? Vector3.up : Vector3.left;

                    panFixture.rotateTransform.transform.localEulerAngles = rotationVector * panTiltPositions[i].X;
                    var tiltFixture = iStageLightFixture.TryGetFixture<LightTiltFixture>();
                }
            }
        }
    }
}