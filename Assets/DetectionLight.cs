using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLight : MonoBehaviour
{
    [SerializeField] bool _instantDie = false;
    PlayerStatus _playerStatus;
    DetectionZone _sphere;

    private void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
        _sphere = GetComponentInChildren<DetectionZone>();
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
            _sphere.transform.position = _playerStatus.transform.position;
        }
    }
}
