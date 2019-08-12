using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float MaxSpeed;
    public float AccelerationTime;
    public float DeAccelerationTime;
    
    private float mSpeed;
    private Vector3 mLastNonzeroInput;

    [Header("Kick")] 
    public float KickDistance;
    public float KickCoolDown;
    public Vector2 KickSpeed;

    private float mLastKickTime;
    
    [Header("Init")]
    [SerializeField] private ControlScheme mControlScheme;
    [SerializeField] private Rigidbody mRigidbody;
    [SerializeField] private Rigidbody mBall;
    [SerializeField] private Transform mCamera;


    private void FixedUpdate()
    {
        #if UNITY_EDITOR
        CheckErrors();
        #endif

        //user input
        var moveInput = GetPlayerInput();

        //transorm to camera view
        var cameraAngle = mCamera.eulerAngles.y;
        var transformedInput = Quaternion.AngleAxis(cameraAngle, Vector3.up) * moveInput;

        //acceleration stuff
        var deltaTime = Time.fixedDeltaTime;
        Vector3 speed;
        if (transformedInput != Vector3.zero)
        {
            mSpeed = Mathf.Clamp01(mSpeed + deltaTime / AccelerationTime);
            speed = transformedInput * mSpeed * MaxSpeed;
            mLastNonzeroInput = transformedInput;
        }
        else
        {
            mSpeed = Mathf.Clamp01(mSpeed - deltaTime / DeAccelerationTime);
            speed = mLastNonzeroInput * mSpeed * MaxSpeed;
        }
        
        //apply new speed
        mRigidbody.velocity = speed;
        
        //kick stuff
        if (Input.GetKey(mControlScheme.Kick))
            if (Time.fixedTime > mLastKickTime + KickCoolDown)
            {
                var directionToBall = mBall.position - mRigidbody.position;
                if (directionToBall.magnitude < KickDistance)
                {
                    directionToBall.y = 0;
                    directionToBall.Normalize();

                    var angle = Vector3.SignedAngle(Vector3.right, directionToBall, Vector3.up);
                    var kickSpeed = Quaternion.AngleAxis(angle, Vector3.up) * KickSpeed;
                    mBall.velocity = kickSpeed;
                }
            }
    }

    public void Reset()
    {
        mSpeed = 0;
        mLastNonzeroInput = Vector3.zero;
    }

    private void CheckErrors()
    {
        if (mControlScheme == null)
        {
            enabled = false;
            throw new UnityException("Null ref to " + nameof(mControlScheme));
        }

        if (mRigidbody == null)
        {
            enabled = false;
            throw new UnityException("Null ref to " + nameof(mRigidbody));
        }

        if (mBall == null)
        {
            enabled = false;
            throw new UnityException("Null ref to " + nameof(mBall));
        }

        if (mCamera == null)
        {
            enabled = false;
            throw new UnityException("Null ref to " + nameof(mCamera));
        }
    }

    private Vector3 GetPlayerInput()
    {
        var moveInput = new Vector3();
        if (Input.GetKey(mControlScheme.Right))
            moveInput.x += 1;
        if (Input.GetKey(mControlScheme.Left))
            moveInput.x -= 1;
        if (Input.GetKey(mControlScheme.Up))
            moveInput.z += 1;
        if (Input.GetKey(mControlScheme.Down))
            moveInput.z -= 1;
        moveInput.Normalize();
        return moveInput;
    }
}
