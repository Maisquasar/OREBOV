using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIButtonAddionalFeature : MonoBehaviour 
{
    [SerializeField] private GameObject _object;
    public void OnPointerClick()
    {
        GeneralMenuFeature.SwitchGameObjectState(_object);

    }
}
