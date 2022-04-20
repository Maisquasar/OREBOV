using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractObject;
using UnityEngine.Events;

public class InteractiveSwitch : InteractiveObject
{
    [Header("Switch")]
    private GameObject _handle;
    [SerializeField]
    private float _handleAmplitude = 0.0442f;
    [SerializeField]
    private float _activationCooldown = 0.0f;
    [SerializeField]
    private float _autoDesactivateTimer = 0.0f;
    [SerializeField]
    private float _handleSpeed = 0.3f;
    [Header("Objects")]
    [SerializeField]
    private UnityEvent _activateEvent = new UnityEvent();
    [SerializeField]
    private UnityEvent _desactivateEvent = new UnityEvent();
    private void Start()
    {
        ObjectType = InteractObjects.Switch;
        _handle = transform.GetChild(0).gameObject;
    }

    protected override void ActiveItem(GameObject player)
    {
        if (_activationCooldown > 0) return;
        if (_objectActive)
        {
            base.DeactiveItem();
            StartCoroutine(desactivateLever());

            _objectActive = false;
        }
        else
        {
            base.ActiveItem(player);
            StartCoroutine(activateLever());
            _objectActive = true;
        }
    }

    protected override void ActiveItem(Enemy enemy)
    {
        if (_activationCooldown > 0) return;
        if (_objectActive)
        {
            base.DeactiveItem();
            StartCoroutine(desactivateLever());
            _objectActive = false;
        }
        else
        {
            base.ActiveItem(enemy);
            StartCoroutine(activateLever());
            _objectActive = true;
        }
    }

    protected override void DeactiveItem()
    {
    }

    private IEnumerator activateLever()
    {
        _activationCooldown = _handleSpeed;
        bool active = false;
        while (_activationCooldown > 0)
        {
            Vector3 tmp = _handle.transform.localPosition;
            tmp.z = 2 * _handleAmplitude * (_activationCooldown / _handleSpeed) - _handleAmplitude;
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z < 0)
            {
                active = true;
                _activateEvent.Invoke();
            }
            _activationCooldown -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        if (_autoDesactivateTimer > 0)
        {
            yield return new WaitForSeconds(_autoDesactivateTimer);
            base.DeactiveItem();
            yield return desactivateLever();
            _objectActive = false;
        }
        yield return null;
    }

    private IEnumerator desactivateLever()
    {
        _activationCooldown = _handleSpeed;
        bool active = false;
        while (_activationCooldown > 0)
        {
            Vector3 tmp = _handle.transform.localPosition;
            tmp.z = -2 * _handleAmplitude * (_activationCooldown / _handleSpeed) + _handleAmplitude;
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z > 0)
            {
                active = true;
                _desactivateEvent.Invoke();
            }
            _activationCooldown -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        yield return null;
    }

}
