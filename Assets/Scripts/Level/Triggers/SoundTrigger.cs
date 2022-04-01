using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : Trigger
{
    [SerializeField] AudioSource sound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            if (sound != null)
            {
                Debug.Log($"Play : {sound.name}");
                sound.Play();
            }
            else
                Debug.Log("No sounds Set");
        }
    }
}
