using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLight : MonoBehaviour
{
    [SerializeField] bool _instantDie = false;

    LightSubType _light;
    PlayerStatus _playerStatus;
    ShadowCaster _shadowCaster;
    [SerializeField] DetectionZone _sphere;

    private void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
        _shadowCaster = _playerStatus.GetComponent<ShadowCaster>();
        _light = GetComponent<LightSubType>();
        if (_instantDie)
            _sphere.DistanceDetection = 0;
        else
            _sphere.DistanceDetection = -1;
        _sphere.LinkToLight = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (_playerStatus.IsInLight)
        {
            List<LightSubType> tmp = _shadowCaster.GetLight();
            for (int i = 0; i < tmp.Count; i++)
                if (tmp[i] == _light)
                    _sphere.transform.position = _playerStatus.transform.position;
        }
    }
}
