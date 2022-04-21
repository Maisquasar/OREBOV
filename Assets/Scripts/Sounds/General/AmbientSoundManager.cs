using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField] private SoundEffectsHandler _sounds;
    [SerializeField] private float _minDelay;
    [SerializeField] private float _maxDelay;
    private AudioSource source;
    float timer = 0;
    float targetTime = 0;

    private void Start()
    {
        targetTime = Random.Range(_minDelay, _maxDelay);
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (source.isPlaying)
        {
            timer = 0.0f;
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > targetTime)
            {
                timer = 0;
                targetTime = Random.Range(_minDelay, _maxDelay);
                _sounds.PlaySound();
            }
        }
    }
}
