using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISoundOptions : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _slider;
    [SerializeField] private string _soundParameter = "MasterVolume";

    [Header("Sounds Settings")]
    [Range(0f, 20f)]
    [SerializeField] private float _maxVolume;

    [Range(-80f, 20f)]
    [SerializeField] private float _startVolume;
    [SerializeField]
    private float _currentVolume;

    private void Start()
    {
        /*
        _currentVolume = _startVolume;
        float generalRatio = Mathf.Abs(_minVolume) + Mathf.Abs(_maxVolume);
        _slider.value = (_currentVolume + Mathf.Abs(_minVolume)) / generalRatio;
        _mixer.SetFloat(_soundParameter, GetVolume(_slider.value));
        */
    }

    public void ChangeVolume()
    {
        _mixer.SetFloat(_soundParameter, GetVolume(_slider.value));
    }

    private float GetVolume(float value)
    {
        if (value <= 0) value = 0.000001f;
        return Mathf.Abs(_maxVolume) * Mathf.Log10(value);
    }


}
