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
    [SerializeField] [Range(0.0f,1.0f)] private float _volume = 1.0f;
    [SerializeField] private bool _randomPlaySound;
    [SerializeField] private bool _randomLeftRightPanning;
    [SerializeField] private bool _playAtStart;
    [SerializeField] private bool _looped;
    [Header("Play Count")]
    [SerializeField] private int _playCount = 1;
    [SerializeField] private int _playCountRandomChances = 0;
    [SerializeField] private float _playDelay = 1.0f;
    [SerializeField] private float _playDelayRandomChances = 0.5f;
    private bool _active = false;

    public bool Active { get { return _active; } }


    public string SoundName { get { return _soundName; } }
    private AudioSource[] _audioSources;
    private bool _multiSources = false;
    private int _indexAudioClip = 0;
    private int _prevAudioClip = 0;

    private void InitComponents()
    {
        _audioSources = GetComponents<AudioSource>();
        if (_audioSources.Length == 0)
        {
            Debug.LogError(" No audio source in the " + gameObject.name + " for the " + _soundName + " sound effect handler . This component disable for play mode ");
        }
        else if (_audioSources.Length > 1)
        {
            _multiSources = true;
        }
    }

    private void CheckComponentIsValid()
    {
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
        if (_active || _audioClipArray.Length == 0) return;
        if (_looped) _active = true;
        if (_playCount == 1 && _playCountRandomChances == 0) playSoundOnce();
        else StartCoroutine(playSoundMultiple());
    }

    public void StopSound()
    {
        if (!_active || _audioClipArray.Length == 0) return;
        StopAllCoroutines();
        _active = false;
        foreach (AudioSource item in _audioSources)
        {
            foreach (AudioClip clip in _audioClipArray)
            {
                if (item.clip == clip)
                {
                    StartCoroutine(soundFadeOut(item, 0.5f));
                    break;
                }
            }
        }
    }

    private int findFirstAudioSource()
    {
        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (!_audioSources[i].isPlaying) return i;
        }
        return _audioSources.Length - 1;
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
        int index = 0;
        if (_multiSources)
        {
            index = findFirstAudioSource();
        }
        if (_randomLeftRightPanning)
        {
            _audioSources[index].panStereo = Random.Range(-1.0f, 1.0f);
        }
        else
        {
            _audioSources[index].panStereo = 0.0f;
        }
        _audioSources[index].clip = _audioClipArray[_indexAudioClip];
        _audioSources[index].outputAudioMixerGroup = _mixer;
        _audioSources[index].loop = _looped;
        _audioSources[index].volume = _volume;
        _audioSources[index].Play();
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

    private IEnumerator soundFadeOut(AudioSource sound, float delay)
    {
        float timer = delay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            sound.volume = timer / delay;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        sound.Stop();
        yield return null;
    }

}
