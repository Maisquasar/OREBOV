using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopterScript2 : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _amplitude = 0.3f;

    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position + new Vector3(0,Mathf.Sin(Time.realtimeSinceStartup*_speed)*_amplitude,0);
    }
}
