using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanelSelection : MonoBehaviour ,ISelectHandler
{

    [SerializeField] private UIOptionsMenu _optionsMenuScript;
    [SerializeField] private GameObject _toActive;
    [SerializeField] private GameObject _toDeactive;
    [SerializeField] private GameObject _firstObjectToSelect;
    public void OnSelect(BaseEventData eventData)
    {
        if (!_toActive.activeSelf)
        {
            _toActive.SetActive(true);
            _toDeactive.SetActive(false);
            _optionsMenuScript.StartCoroutine(_optionsMenuScript.SetSelectedObject(_firstObjectToSelect));
        }

    }



 

}
