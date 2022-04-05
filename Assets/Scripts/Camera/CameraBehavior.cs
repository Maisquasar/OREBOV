using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{




    [SerializeField]
    private GameObject _playerGO;
    [SerializeField]
    private Rect _windown;

    [SerializeField]
    private Vector3 _windownCenter;
    [SerializeField]
    private Vector3 _windownSize;
    [SerializeField]
    private Vector3 _windownOffset;

    private Vector3 _windownOrigin;



    [SerializeField]
    [Range(0f,10f)]
    private float _camSpeed;

    private bool _inScreen;


    [Header("Debug Camera")]
    [SerializeField]
    private bool _activeCameraDebug;


    public void Start()
    {
       
        _windownOrigin = _windownCenter +(_windownSize/2f);

    }

    private void FixedUpdate()
    {
        _windownCenter = transform.position + _windownOffset;
        _windownCenter.z = _playerGO.transform.position.z;
        _windownOrigin = _windownCenter - (_windownSize / 2f);
        _inScreen = WindownCamContains(_playerGO.transform.position);
        Debug.Log(_inScreen);



        if (!_inScreen)
        {
            Vector3 target = new Vector3(_playerGO.transform.position.x, 0, 0f) - new Vector3(_windownCenter.x, 0, 0f);
            transform.position += target.normalized * _camSpeed * Time.deltaTime;

        }
    }


    private bool IsPlayerInCameraWindown()
    {
        Vector3 screnPos = Camera.main.WorldToScreenPoint(_playerGO.transform.position);
        return _windown.Contains(screnPos);
    }

   


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_windownOrigin, _windownOrigin + new Vector3(_windownSize.x, 0f, 0f));
        Gizmos.DrawLine(_windownOrigin + new Vector3(_windownSize.x, 0f, 0f), _windownOrigin + _windownSize);
        Gizmos.DrawLine(_windownOrigin + _windownSize, _windownOrigin + new Vector3(0, _windownSize.y, 0f));
        Gizmos.DrawLine(_windownOrigin + new Vector3(0, _windownSize.y, 0f),_windownOrigin);

    }


    private bool WindownCamContains(Vector3 position)
    {

        if (position.x < _windownOrigin.x || position.x > _windownOrigin.x + _windownSize.x) return false;
        if (position.y < _windownOrigin.y || position.y > _windownOrigin.y + _windownSize.y) return false;


        return true;
    }



}
