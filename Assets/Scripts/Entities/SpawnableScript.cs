using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableScript : MonoBehaviour
{
    public Rigidbody body;
    public float Angle;
    public float AngleDeltaMax;
    public float MinSpeed;
    public float MaxSpeed;

    bool movable = true;
    // Start is called before the first frame update
    void Start()
    {
        float dir = Mathf.Deg2Rad * (Angle + Random.Range(-AngleDeltaMax, AngleDeltaMax));
        float speed = Random.Range(MinSpeed, MaxSpeed);
        body.AddForce(new Vector3(Mathf.Cos(dir), Mathf.Sin(dir), 0) * speed, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (movable && !other.isTrigger)
        {
            Component t = other.GetComponent(typeof(EntityController));
            if (t == null)
            {
                t = other.GetComponent(typeof(CollectableScript));
                if (t == null)
                {
                    t = other.GetComponent(typeof(MovableObject));
                    if (t != null)
                    {
                        GameObject tmp = new GameObject("CoinContainer");
                        tmp.transform.SetParent(t.transform,true);
                        transform.SetParent(tmp.transform,true);
                    }
                    body.velocity = new Vector3();
                    body.useGravity = false;
                    body.isKinematic = true;
                    movable = false;
                }
            }
        }
    }
}
