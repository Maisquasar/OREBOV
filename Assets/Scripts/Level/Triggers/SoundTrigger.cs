using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : Trigger
{
    [SerializeField] AudioSource _sound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            if (_sound != null)
            {
                Debug.Log($"Play : {_sound.name}");
                _sound.Play();
            }
            else
                Debug.Log("No sounds Set");
        }
    }
}
