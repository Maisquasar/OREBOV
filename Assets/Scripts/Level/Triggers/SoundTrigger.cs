using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundTrigger : Trigger
{
    [SerializeField] private AudioSource _sound;
    [SerializeField] private bool _playOnce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>()) PlaySoundTrigger();
    }

    private void PlaySoundTrigger()
    {
        if (_sound != null)
        {
            _sound.Play();
            if (_playOnce) Destroy(gameObject);
        }
    }
}

