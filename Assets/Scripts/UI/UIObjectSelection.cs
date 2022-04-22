using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class UIObjectSelection : MonoBehaviour , ISelectHandler, IDeselectHandler
{

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color32 _selected;
    [SerializeField] private Color32 _deselected;


    private void Start()
    {
        InitComponents();
    }

    private void InitComponents()
    {
        _text.color = _deselected;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _text.color = _selected;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _text.color = _deselected;
    }
}
