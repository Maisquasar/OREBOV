using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSwitch : InteractiveObject
{
    [Header("Light")]
    [SerializeField]
    private Light[] _lightConnect = new Light[0];
    [SerializeField]


    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        for (int i = 0; i < _lightConnect.Length; i++)
        {
            _lightConnect[i].gameObject.SetActive(!_lightConnect[i].gameObject.activeSelf);
        }
    }
      

    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        for (int i = 0; i < _lightConnect.Length; i++)
        {
            _lightConnect[i].gameObject.SetActive(!_lightConnect[i].gameObject.activeSelf);
        }
    }

}
