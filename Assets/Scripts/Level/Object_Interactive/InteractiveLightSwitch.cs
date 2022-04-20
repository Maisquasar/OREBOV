using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractObject;

public class InteractiveLightSwitch : InteractiveObject
{
    [Header("Switch")]
    [SerializeField] private float _handleAmplitude = 0.0442f;
    [SerializeField] private float _activationCooldown = 0.0f;
    [SerializeField] private float _handleSpeed = 0.3f;

    [Header("Light")]
    [SerializeField] private Light[] _lightConnect = new Light[0];

    private GameObject _handle;


    protected override void Start()
    {
        base.Start();
        ObjectType = InteractObjects.Switch;
        _handle = transform.GetChild(0).gameObject;
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
            StartCoroutine(ActivateLever(true ));
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
            tmp.z = sign * 2 * _handleAmplitude * (_activationCooldown / _handleSpeed) - (sign*  _handleAmplitude);
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z < 0)
            {
                active = true;
                ToggleLamps();
            }
            _activationCooldown -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        yield return null;
    }

    private void ToggleLamps()
    {
        for (int i = 0; i < _lightConnect.Length; i++)
        {
            _lightConnect[i].gameObject.SetActive(!_lightConnect[i].gameObject.activeSelf);
        }
    }

}
