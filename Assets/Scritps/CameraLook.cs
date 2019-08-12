using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public Transform Ball;

    [Header("Zoom settings")] 
    public float FovMin;
    public float FovMax;
    public float DistMin;
    public float DistMax;

    [Header("LookSpeed")]
    public float LerpCoeff;

    [Header("Init")]
    [SerializeField] private Camera mCamera;

    void Update()
    {
        #if UNITY_EDITOR
        CheckErrors();
        #endif
        
        if (Ball == null)
            return;
        
        var direction = Ball.position - transform.position;
        
        //Zoom
        var distCoeff = Mathf.InverseLerp(DistMin, DistMax, direction.magnitude);
        mCamera.fieldOfView = Mathf.Lerp(FovMax, FovMin, distCoeff);
        
        //Rotation
        var newRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, LerpCoeff);
    }

    private void CheckErrors()
    {
        if (mCamera == null)
        {
            enabled = false;
            throw new UnityException("Null ref to " + nameof(mCamera));
        }
    }
}
