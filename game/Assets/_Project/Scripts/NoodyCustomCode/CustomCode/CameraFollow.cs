using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOOD.NoodCamera
{ 
    public class CameraFollow : MonoBehaviorInstance<CameraFollow>
    {
        [SerializeField] float smoothTime = 2;
        [SerializeField] string targetTag = "Player";
        [SerializeField] Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] bool isFollow = true;
        Transform targetTransform;

        private void LateUpdate()
        {
            if (isFollow) FollowPlayer();
        }

        void FollowPlayer()
        {
            if (!targetTransform && GameObject.FindGameObjectWithTag(targetTag)) targetTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
            if (targetTransform)
                NOOD.NoodyCustomCode.LerpSmoothCameraFollow(this.gameObject, smoothTime, targetTransform, offset);
        }
    }
}
