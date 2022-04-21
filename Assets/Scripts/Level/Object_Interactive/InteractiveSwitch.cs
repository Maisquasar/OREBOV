using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractObject;
using UnityEngine.Events;

public class InteractiveSwitch : InteractiveObject
{
    [Header("Switch")]
    [SerializeField] private GameObject _handle;
    [SerializeField] private float _handleAmplitude = 0.0442f;
    [SerializeField] private float _activationCooldown = 0.0f;
    [SerializeField] private float _autoDesactivateTimer = 0.0f;
    [SerializeField] private float _handleSpeed = 0.3f;
    
    [Header("Objects")]
    [SerializeField]private UnityEvent _activateEvent = new UnityEvent();
    [SerializeField] private UnityEvent _desactivateEvent = new UnityEvent();
    private GameObject _handle;
    protected override void Start()
    {
        ObjectType = InteractObjects.Switch;
        if (!_handle) _handle = transform.GetChild(0).gameObject;
    }

    protected override void ActiveItem(GameObject player)
    {
        if (_activationCooldown > 0) return;
        if (ObjectActive)
        {
            base.DeactiveItem();
            StartCoroutine(ActivateLever(false));
            ObjectActive = false;
        }
        else
        {
            base.ActiveItem(player);
            StartCoroutine(ActivateLever(true));
            ObjectActive = true;
        }
    }

    protected override void ActiveItem(Enemy enemy)
    {
        if (_activationCooldown > 0) return;
        if (ObjectActive)
        {
            base.DeactiveItem();
            StartCoroutine(ActivateLever(false));
            ObjectActive = false;
        }
        else
        {
            base.ActiveItem(enemy);
            StartCoroutine(ActivateLever(true));
            ObjectActive = true;
        }
    }

    private IEnumerator ActivateLever(bool state)
    {
        _activationCooldown = _handleSpeed;
        bool active = false;
        int sign = state == true ? 1 : -1;
        while (_activationCooldown > 0)
        {
            Vector3 tmp = _handle.transform.localPosition;
            tmp.z = sign * 2 * _handleAmplitude * (_activationCooldown / _handleSpeed) - (sign * _handleAmplitude);
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z < 0)
            {
                active = true;
                if (state) _activateEvent.Invoke();
                else _desactivateEvent.Invoke();
            }
            _activationCooldown -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        if (_autoDesactivateTimer > 0)
        {
            _activationCooldown = _autoDesactivateTimer;
            yield return new WaitForSeconds(_autoDesactivateTimer);
            base.DeactiveItem();
            yield return desactivateLever();
            _objectActive = false;
        }
        yield return null;
    }

   

}
