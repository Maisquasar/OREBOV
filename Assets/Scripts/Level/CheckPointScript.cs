using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    public Material ActiveMaterial;
    public MeshRenderer FlagSkin;
    public ParticleSystem ParticleEmitter;
    bool Active = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Active && !other.isTrigger)
        {
            Component t = other.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                ((PlayerController)t).RespawnPoint = transform.position + new Vector3(-1,3,0);
                if (FlagSkin && ActiveMaterial) FlagSkin.material = ActiveMaterial;
                if (ParticleEmitter) ParticleEmitter.Play();
                Active = false;
            }
        }
    }
}
