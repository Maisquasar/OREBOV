using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagAnimate : MonoBehaviour
{
    float BaseRotation;
    float DeltaRotation;
    // Start is called before the first frame update
    void Start()
    {
        BaseRotation = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        DeltaRotation = 6 * Mathf.Sin(Time.time*9.1501f) + 2.5f * Mathf.Sin(Time.time*4.984f) + 4 * Mathf.Sin(Time.time*3.7845f);
        transform.eulerAngles = new Vector3(0, BaseRotation + DeltaRotation, 0);
    }
}
