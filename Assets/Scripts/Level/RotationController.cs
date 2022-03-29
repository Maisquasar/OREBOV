using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{

    public float AngularVelocity = 5.0f;
    public float AngularDelta = 0.0f;
    public float RadiusX = 1.0f;
    public float RadiusY = 1.0f;
    public bool ShouldPivotObject = false;

    private float angle = 0;
    // Start is called before the first frame update
    void Start()
    {
        AngularVelocity = AngularVelocity / 180.0f * Mathf.PI;
        angle = AngularDelta / 180.0f * Mathf.PI;
    }

    // Update is called once per frame
    void Update()
    {
        angle += AngularVelocity * Time.deltaTime;
        transform.localPosition = new Vector3(Mathf.Cos(angle) * RadiusX, 0, Mathf.Sin(angle) * RadiusY);
        if (ShouldPivotObject) transform.Rotate(0,-AngularVelocity*Time.deltaTime*180.0f/Mathf.PI,0);
    }
}
