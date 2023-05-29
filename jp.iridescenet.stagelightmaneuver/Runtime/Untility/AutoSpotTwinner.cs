using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageLightManeuver
{
    [ExecuteAlways]
    public class AutoSpotTwinner : MonoBehaviour
    {

        public Transform target;
        private Vector3 panVelocity;
        [Range(0.001f, 0.3f)] public float speed = 1f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            var targetRotation = Quaternion.LookRotation(target.position - transform.position);
            // calculate angle SmoothDampAngle
            var angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, ref panVelocity.y, speed);
            var angleX = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotation.eulerAngles.x, ref panVelocity.x, speed);
            var angleZ = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.eulerAngles.z, ref panVelocity.z, speed);
            transform.eulerAngles = new Vector3(angleX, angleY, angleZ);
            
            panVelocity = new Vector3( angleX, angleY, angleZ);
            // transform.rotation = Quaternion.LookRotation(target.position - transform.position);
            
            // calculate the rotation needed to point at the target smoothdamp    
        }
    }
}