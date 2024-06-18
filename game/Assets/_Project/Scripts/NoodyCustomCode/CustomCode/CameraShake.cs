using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOOD.NoodCamera
{ 
    public class CameraShake : MonoBehaviorInstance<CameraShake>
    {
        #region Components
        #endregion

        #region Stats
        [SerializeField] float duration = 0.2f, magnitude = 0.02f;
        [SerializeField] float explodeMagnitude = 0.1f;

        [SerializeField] Vector3 offset;
        [SerializeField] bool isShake;
        [SerializeField] bool isHeavyShake;
        #endregion

        private void Update()
        {
            if (isShake) Shake();
            if (isHeavyShake) HeaveShake();
        }

        public void Shake()
        {
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, magnitude));
            isShake = false;
        }

        public void HeaveShake()
        {
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, explodeMagnitude));
            isHeavyShake = false;
        }
    }
}
