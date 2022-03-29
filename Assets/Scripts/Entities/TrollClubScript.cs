using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollClubScript : MonoBehaviour
{
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

    private void OnCollisionEnter(Collision other)
    {
        if (Active && !other.collider.isTrigger)
        {
            Component t = other.collider.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                ((PlayerController)t).DamagePlayer();
                return;
            }
            t = other.collider.GetComponent(typeof(EntityController));
            if (t != null)
            {
                ((EntityController)t).KillEntity();
            }
        }
    }
}
