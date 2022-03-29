using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallScript : LootingEntity
{
    public float LifeTime = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime < 0 || transform.position.y < 0)
        {
            base.KillEntity();
            Destroy(transform.gameObject);
        }
    }
}
