using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsHandler : MonoBehaviour
{
    [SerializeField] private string _soundName;
    [SerializeField] private AudioClip[] _audioClipArray = new AudioClip[0];
    [SerializeField] private AudioMixerGroup _mixer;
    [SerializeField] private bool _randomPlaySound;
    [SerializeField] private bool _playAtStart;
    [Header("Play Count")]
    [SerializeField] private int _playCount = 1;
    [SerializeField] private int _playCountRandomChances = 0;
    [SerializeField] private float _playDelay = 1.0f;
    [SerializeField] private float _playDelayRandomChances = 0.5f;
    private bool _active = false;


    private AudioSource _audioSource;
    private int _indexAudioClip = 0;
    private int _prevAudioClip = 0;

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
        if (_audioClipArray.Length == 1)
        {
            _randomPlaySound = false;
        }
    }

    private void Start()
    {
        CheckComponentIsValid();
        InitComponents();
        if (_playAtStart) PlaySound();

    }

    private int GetRandomIndex()
    {
        int number = _prevAudioClip;
        while (number == _prevAudioClip)
        {
            number = Random.Range(0, _audioClipArray.Length);
        }

        return number;
    }

    public void PlaySound()
    {
        if (_active) return;
        if (_playCount == 1 && _playCountRandomChances == 0) playSoundOnce();
        else StartCoroutine(playSoundMultiple());
    }

    public void PlaySound(bool loop)
    {
        if (_active) return;
        _audioSource.loop = loop;
        if (_playCount == 1 && _playCountRandomChances == 0) playSoundOnce();
        else StartCoroutine(playSoundMultiple());
    }

    public void StopSound()
    {
        _audioSource.loop = false;
        _audioSource.Stop();
    }




    private void playSoundOnce()
    {
        if (_randomPlaySound)
        {
            _prevAudioClip = _indexAudioClip;
            _indexAudioClip = GetRandomIndex();
        }
        else
        {
            _indexAudioClip = _indexAudioClip + 1 == _audioClipArray.Length ? 0 : _indexAudioClip++;
        }

        _audioSource.clip = _audioClipArray[_indexAudioClip];
        _audioSource.outputAudioMixerGroup = _mixer;
        _audioSource.Play();
    }

    private IEnumerator playSoundMultiple()
    {
        _active = true;
        int count = _playCount + Random.Range(0, _playCountRandomChances);
        while (count > 0)
        {
            playSoundOnce();
            count--;
            yield return new WaitForSeconds(_playDelay + Random.Range(0, _playDelayRandomChances));
        }
        _active = false;
        yield return null;
    }

}
