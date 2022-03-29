using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthPointScript : CollectableScript
{
    public float MovementSpeed = 90.0f;
    public float MovementAmplitude = 0.2f;

    float DeltaPos;
    // Start is called before the first frame update
    void Start()
    {
        DeltaPos = Random.Range(0.0f, 360.0f);
    }

    // Update is called once per frame
    void Update()
    {
        DeltaPos += Mathf.Deg2Rad * Time.deltaTime * MovementSpeed;
        transform.localPosition += new Vector3(0, Mathf.Cos(DeltaPos)*MovementAmplitude,0)*Time.deltaTime;
    }

    public override void Collect(PauseMenuScript controller)
    {
        controller.OnCollectHealth();
        Destroy(gameObject);
    }
}
