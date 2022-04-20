    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionSwitchEvent : InteractiveObject
{
    [SerializeField] private bool _switchState;
    [SerializeField] private UnityEvent _switchEvent;
    [SerializeField] private float _activationCooldown = 0.3f;

    private float _activationTimer = 0f;


    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        if (_activationTimer > 0) return;
        _switchState = !_switchState;
        _switchEvent.Invoke();
        StartCoroutine(ActiveSwitch());
    }

    private IEnumerator ActiveSwitch()
    {
        _activationTimer = _activationCooldown;
        while (_activationTimer > 0)
        {
            _activationTimer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        yield return null;

    }
}

