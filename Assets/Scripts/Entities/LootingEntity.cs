using System.Collections.Generic;
using UnityEngine;

public abstract class LootingEntity : EntityController
{
    public GameObject BaseObject;
    public GameObject RareObject;

    public int MinCount = 0;
    public int MaxCount = 1;
    public float RareChance = 0.05f;

    public override void KillEntity()
    {
        if (Random.value <= RareChance)
        {
            Instantiate(RareObject,transform.parent.parent).transform.position = transform.position + new Vector3(0,0.5f,0);
        }
        else
        {
            int count = Random.Range(MinCount, MaxCount);
            for (int i = 0; i < count; i++)
            {
                Instantiate(BaseObject,transform.parent.parent).transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }
        }
    }
}
