using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSwitch : InteractiveObject
{
    [Header("Light")]
    [SerializeField]
    private Light _lightConnect;
    [SerializeField]
    private bool _lightState = false;


    protected override void ActiveItem()
    {
        base.ActiveItem();
        _lightConnect.enabled = true;
    }

    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        _lightConnect.enabled = false;
    }

    private void Start()
    {
        _objectActive = _lightState;
        _lightConnect.enabled = _lightState;
    }
}
