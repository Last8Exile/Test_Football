using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    public event Action<Collider> OnCollisionEnter;
    
    private void OnTriggerEnter(Collider other)
    {
        OnCollisionEnter?.Invoke(other);
    }
}
