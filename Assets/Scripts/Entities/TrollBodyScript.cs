using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollBodyScript : MonoBehaviour
{
    public bool Touched = false;
    public bool Active = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    public Collider getHitBox()
    {
        foreach (Component t in transform.GetComponents(typeof(Collider)))
        {
            Collider col = (Collider)t;
            if (!col.isTrigger) return col;
        }
        return null;
    }

    public Collider getTrigger()
    {
        foreach (Component t in transform.GetComponents(typeof(Collider)))
        {
            Collider col = (Collider)t;
            if (col.isTrigger) return col;
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Component t = other.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                Touched = true;
                PlayerController player = (PlayerController)t;
                player.Body.velocity = new Vector3();
                player.Body.AddForce(new Vector3(-100, 300, 0), ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (Active && !other.collider.isTrigger)
        {
            Component t = other.collider.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                ((PlayerController)t).DamagePlayer();
            }
        }
    }
}
