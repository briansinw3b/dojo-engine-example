using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch(mode)
        {
            case Mode.LookAt:
                this.transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                this.transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                this.transform.forward = Camera.main.transform.position;
                break;
            case Mode.CameraForwardInverted:
                this.transform.forward = -Camera.main.transform.position;
                break;
        }
    }
}
