using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsHandler : MonoBehaviour
{
    [SerializeField]
    private string _soundName;

    [SerializeField]
    private AudioClip[] _audioClipArray = new AudioClip[0];
    private AudioSource _audioSource;

    private int _indexAudioClip = 0;
    [SerializeField]
    private bool _randomPlaySound;

    private void InitComponents()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void CheckComponentIsValid()
    {
        if (_audioClipArray.Length == 0)
        {
            Debug.LogError(" No sound in the " + gameObject.name + " for the " + _soundName + " sound effect handler . This component disable for play mode ");
            this.enabled = false;
        }
    }


    private void Start()
    {
        CheckComponentIsValid();
        InitComponents();

    }

    private int GetRandomIndex()
    {
        return Random.Range(0, _audioClipArray.Length);
    }

    public void PlaySound()
    {
        if (_randomPlaySound)
        {
            _indexAudioClip = GetRandomIndex();
        }
        else
        {
            _indexAudioClip = _indexAudioClip + 1 == _audioClipArray.Length ? 0: _indexAudioClip++ ;
           
        }

        _audioSource.clip = _audioClipArray[_indexAudioClip];
        _audioSource.Play();
    }


}
