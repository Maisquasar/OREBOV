using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakGround : MonoBehaviour
{
    [SerializeField] private float _minimunFallSpeed;

    private Collider _colliderGO;

    private void Start()
    {
        InitComponent();
    }
    private void InitComponent()
    {
        _colliderGO = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<InteractiveBox>())
        {
            bool isBreak =  collision.relativeVelocity.y < -_minimunFallSpeed;
           
            if(isBreak) { _colliderGO.enabled = false; }
        }
    }

}
