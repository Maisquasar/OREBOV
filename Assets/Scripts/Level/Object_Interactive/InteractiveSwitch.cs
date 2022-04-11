using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractObject;

public class InteractiveSwitch : InteractiveObject
{
    [Header("Switch")]
    private GameObject _handle;
    [SerializeField]
    private float _handleAmplitude = 0.0442f;
    [SerializeField]
    private float _handleSpeed = 0.3f;
    [Header("Light")]
    [SerializeField]
    private Light[] _lightConnect = new Light[0];
    [SerializeField]
    private void Start()
    {
        ObjectType = InteractObjects.Switch;
        _handle = transform.GetChild(0).gameObject;
    }

    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        StartCoroutine(activateLever());
    }
    
    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        StartCoroutine(desactivateLever());
    }

    private IEnumerator activateLever()
    {
        float timer = _handleSpeed;
        bool active = false;
        while (timer > 0)
        {
            Vector3 tmp = _handle.transform.localPosition;
            tmp.z = 2 * _handleAmplitude * (timer/_handleSpeed) - _handleAmplitude;
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z < 0)
            {
                active = true;
                toggleLamps();
            }
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        yield return null;
    }

    private IEnumerator desactivateLever()
    {
        float timer = _handleSpeed;
        bool active = false;
        while (timer > 0)
        {
            Vector3 tmp = _handle.transform.localPosition;
            tmp.z = -2 * _handleAmplitude * (timer / _handleSpeed) + _handleAmplitude;
            _handle.transform.localPosition = tmp;
            if (!active && tmp.z > 0)
            {
                active = true;
                toggleLamps();
            }
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        yield return null;
    }

    private void toggleLamps()
    {
        for (int i = 0; i < _lightConnect.Length; i++)
        {
            _lightConnect[i].gameObject.SetActive(!_lightConnect[i].gameObject.activeSelf);
        }
    }

}
