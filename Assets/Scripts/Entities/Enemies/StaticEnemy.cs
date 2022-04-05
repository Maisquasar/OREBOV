using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Entity
{
    private Player player;
    [SerializeField] Trigger DetectionZone;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
