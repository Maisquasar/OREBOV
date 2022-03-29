using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpController : MonoBehaviour
{
    public WarpController destination;
    public float warpCooldown = 3.0f;
    private float cooldown = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (cooldown > 0.0f) cooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cooldown <= 0.0f && other.CompareTag("Player"))
        {
            other.transform.position = destination.transform.position;
            cooldown = warpCooldown;
            destination.cooldown = warpCooldown;
        }
    }
}
