using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : SwitchableObjects
{
    Pivot _pivot;

    [SerializeField] public bool IsOpen;
    [SerializeField] public float Angle;
    // Start is called before the first frame update
    void Start()
    {
        _pivot = GetComponentInChildren<Pivot>();
    }

    override public void Activate()
    {
        IsOpen = !IsOpen;
        transform.RotateAround(_pivot.transform.position, Vector3.up, IsOpen ? Angle : -Angle);
    }
}
