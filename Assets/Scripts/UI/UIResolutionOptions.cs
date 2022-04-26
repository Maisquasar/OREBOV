using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UIResolutionOptions : MonoBehaviour
{
    [SerializeField] private GameVisualSettings _visualSetting;
    [SerializeField] private TextMeshProUGUI _text;
    void Start()
    {
        ShowResolution();
    }

   
    public void ShowResolution()
    {
        _text.text = _visualSetting.GameResolutions[_visualSetting.CurrentResolution].width.ToString("F0") + " x " + _visualSetting.GameResolutions[_visualSetting.CurrentResolution].height.ToString("F0");
    }
}
