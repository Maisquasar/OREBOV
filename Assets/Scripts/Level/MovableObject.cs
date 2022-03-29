using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    List<Rigidbody> TouchedBodies;
    Vector3 oldPos;
    // Start is called before the first frame update
    void Start()
    {
        TouchedBodies = new List<Rigidbody>();
        oldPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaPos = transform.position - oldPos;
        TouchedBodies.RemoveAll(element => element == null);
        foreach (Rigidbody body in TouchedBodies)
        {
            body.transform.position += deltaPos;
        }
        oldPos = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger)
        {
            Component t = collision.collider.GetComponent(typeof(Rigidbody));
            if (t != null)
            {
                Rigidbody other = (Rigidbody)t;
                if (!other.isKinematic)
                {
                    TouchedBodies.Add(other);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.isTrigger)
        {
            Component t = collision.collider.GetComponent(typeof(Rigidbody));
            if (t != null)
            {
                Rigidbody other = (Rigidbody)t;
                if (!other.isKinematic)
                {
                    TouchedBodies.Remove(other);
                }
            }
        }
    }
}
