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

    private void Start()
    {
        float actualValue;
        _mixer.GetFloat(_soundParameter, out actualValue);
        _slider.value = GetReverseVolume(actualValue);
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

    private float GetReverseVolume(float value)
    {
        return Mathf.Pow(10,value/Mathf.Abs(_maxVolume));
    }
}
