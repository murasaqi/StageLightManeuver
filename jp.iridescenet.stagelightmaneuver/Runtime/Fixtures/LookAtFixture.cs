using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StageLightManeuver
{
    public class LookAtFixture:StageLightFixtureBase
    {
        
        public LightPanFixture panFixture;
        public LightTiltFixture tiltFixture;
        public List<Transform> lookAtTransforms = new List<Transform>();
        public int lookAtTransformIndex = 0;
        public float lookAtWeight = 1f;
        public Vector3 resultAngle = Vector3.zero;
        
        public Vector3 panoffset = Vector3.zero;
        public Vector3 tiltOffset = Vector3.zero;


        public LookAtConstraint lookAtDummy;
        private void Start()
        {
            Init();
        }
        
        public override void Init()
        {
            base.Init();
            PropertyType = typeof(RotationProperty);
            InitLookAt();
        }


        public void InitLookAt()
        {
            if (lookAtDummy == null)
            {
                var go = new GameObject("LookAtDummy");
                go.transform.SetParent(transform);
                lookAtDummy = go.AddComponent<LookAtConstraint>();
                
            }
        }
        public override void EvaluateQue(float time)
        {
            
            resultAngle = Vector3.zero;
            lookAtTransformIndex = 0;
            while (stageLightDataQueue.Count > 0)
            {
                var queueData = stageLightDataQueue.Dequeue();
                var stageLightBaseProperties = queueData.TryGet<TimeProperty>() as TimeProperty;
                var lookAtProperty = queueData.TryGet<LookAtProperty>() as LookAtProperty;

                if (lookAtProperty == null || stageLightBaseProperties == null)
                    return;

                var normalizedTime = GetNormalizedTime(time, queueData, typeof(LookAtProperty));

                lookAtTransformIndex = queueData.weight >= 0.5f ? lookAtProperty.lookAtIndex.value : lookAtTransformIndex;
                // calculate the angle between this transform and the target
                if (lookAtTransforms.Count > 0 && lookAtTransformIndex < lookAtTransforms.Count)
                {
                    var lookAtTransform = lookAtTransforms[lookAtTransformIndex];
                    lookAtDummy.constraintActive = true;
                    if (lookAtDummy.sourceCount == 0)
                    {
                        lookAtDummy.AddSource(new ConstraintSource { sourceTransform = lookAtTransform, weight = 1f });
                        // lookAtDummy.SetSource(0, new ConstraintSource {sourceTransform = lookAtTransform, weight = 1f});
                    }
                    else
                    {
                        lookAtDummy.SetSource(0, new ConstraintSource {sourceTransform = lookAtTransform, weight = 1f});
                    }
                    var targetDir = lookAtTransform.position - transform.position;
                    var forward = transform.forward;
                    var angle = Vector3.Angle(targetDir, forward);
                    var axis = Vector3.Cross(forward, targetDir);
                    var rotation = Quaternion.AngleAxis(angle, axis);
                    var rotationVector = rotation.eulerAngles; 
                    resultAngle += rotationVector * queueData.weight;
                }
                   
                
                
            }
        }
        
        public override void UpdateFixture()
        {
            if(lookAtDummy == null)
                InitLookAt();

            if (panFixture)
            {
                panFixture.rotateTransform.localEulerAngles =
                    panoffset + new Vector3(lookAtDummy.transform.localEulerAngles.x * panFixture.rotationVector.x,
                        lookAtDummy.transform.localEulerAngles.y * panFixture.rotationVector.y,
                        lookAtDummy.transform.localEulerAngles.z * panFixture.rotationVector.z);
            }

            if (tiltFixture)
            {
                // Debug.Log(tiltFixture);
                tiltFixture.rotateTransform.localEulerAngles =
                    tiltOffset + new Vector3(lookAtDummy.transform.localEulerAngles.x * tiltFixture.rotationVector.x,
                        lookAtDummy.transform.localEulerAngles.y * tiltFixture.rotationVector.y,
                        lookAtDummy.transform.localEulerAngles.z * tiltFixture.rotationVector.z);
            }
        }
    }
}