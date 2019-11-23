using System;
using UnityEngine;
using System.Collections;


namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Camera work. Follow a target
    /// </summary>
    public class CameraWork : MonoBehaviour
    {

        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 150.0f;
        

        private int xmax = 320;
        private int zmax = 220;

        // cached transform of the target
        Transform cameraTransform;


        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool isFollowing;
        

        void Start()
        {


        }


        /// <summary>
        /// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution. For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.
        /// </summary>
        void LateUpdate()
        {
            // The transform target may not destroy on level load,
            // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }
            // only follow is explicitly declared
            if (isFollowing)
            {
                Apply();
            }
        }

        

        /// <summary>
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// </summary>
        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            Apply	();
        }


        /// <summary>
        /// Follow the target smoothly
        /// </summary>
        void Apply()
        {
            Vector3 targetCenter = transform.position;
            targetCenter.x = (targetCenter.x < -xmax) ? -xmax : targetCenter.x;
            targetCenter.x = (targetCenter.x > xmax) ? xmax : targetCenter.x;
            targetCenter.z = (targetCenter.z < -zmax) ? -zmax : targetCenter.z;
            targetCenter.z = (targetCenter.z > zmax) ? zmax : targetCenter.z;


            float currentHeight = cameraTransform.position.y;
            cameraTransform.position = targetCenter;
            cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z );
            // Always look at the target
            
            
            //SetUpRotation(targetCenter);
        }

        

        
    }
}