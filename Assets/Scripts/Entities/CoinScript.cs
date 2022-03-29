using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinScript : CollectableScript
{
    public float RotationSpeed = 90.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, Random.Range(0.0f,360.0f), 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,RotationSpeed*Time.deltaTime,0));
    }

    public override void Collect(PauseMenuScript controller)
    {
        controller.OnCollectCoin();
        Destroy(gameObject);
    }
}
