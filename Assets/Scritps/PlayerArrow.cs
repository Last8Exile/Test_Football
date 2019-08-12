using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] private Rigidbody mPlayer;
    [SerializeField] private Camera mCamera;
    [SerializeField] private RectTransform mArrow;

    void Update()
    {
        #if UNITY_EDITOR
        CheckErrors();
        #endif
        
        Vector2 screenPos = mCamera.WorldToScreenPoint(mPlayer.position);
        var screenCenter = new Vector2(Screen.width, Screen.height) / 2;
        screenPos -= screenCenter; // (0,0) is screenCenter
        
        var isOffScreen = !IsInsideScreen(screenPos);
        mArrow.gameObject.SetActive(isOffScreen);
        if (isOffScreen)
        {
            //postion
            var horizontalCoeff = Mathf.Abs(InverseLerpUnclamped(0, Screen.width, screenPos.x*2));
            var verticalCoeff = Mathf.Abs(InverseLerpUnclamped(0, Screen.height, screenPos.y*2));
            var resizeCoeff = Mathf.Max(horizontalCoeff, verticalCoeff);
            var cappedPos = new Vector2(screenPos.x / resizeCoeff, screenPos.y / resizeCoeff);
            mArrow.position = cappedPos + screenCenter; //world position assigned!
            
            //rotation
            var angle = Vector2.SignedAngle(Vector2.left, cappedPos);
            mArrow.rotation = Quaternion.Euler(0,0,angle);
        }
    }
    
    private void CheckErrors()
    {
        if (mPlayer == null)
        {
            enabled = false;
            throw new UnityException("Null ref to" + nameof(mPlayer));
        }
        if (mCamera == null)
        {
            enabled = false;
            throw new UnityException("Null ref to" + nameof(mCamera));
        }
        if (mArrow == null)
        {
            enabled = false;
            throw new UnityException("Null ref to" + nameof(mArrow));
        }
    }

    private bool IsInsideScreen(Vector2 pos)
    {
        if (Mathf.Abs(pos.x)*2 > Screen.width)
            return false;
        if (Mathf.Abs(pos.y)*2 > Screen.height)
            return false;
        return true;
    }

    private float InverseLerpUnclamped(float a, float b, float value)
    {
        return (value - a) / (b - a);
    }
}
