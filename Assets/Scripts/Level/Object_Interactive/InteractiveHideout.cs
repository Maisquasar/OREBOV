using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveHideout : InteractiveObject
{
    private PlayerStatus _playerStatus;
    private MeshRenderer _playerMeshRenderer;

    [Header("Hideout Setting")]
    [SerializeField] private float _playerFadingTime;
    private float _playerFadingTimer;

    private bool _isFading;


    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        GetPlayerComponent();
        StartCoroutine(PlayerFading(true));


    }

    private void GetPlayerComponent()
    {
        if (_playerStatus == null)
            _playerStatus = _playerGO.GetComponent<PlayerStatus>();
        if (_playerMeshRenderer == null)
            _playerMeshRenderer = _playerGO.GetComponent<MeshRenderer>();
    }

    private IEnumerator PlayerFading(bool active)
    {
        float startAlpha = active == true ? 1f : 0f;
        float endAlpha = active == false ? 1f : 0f;
        _isFading = true;
        Color matColor = _playerMeshRenderer.material.color;

        while (_playerFadingTimer < _playerFadingTime)
        {

            matColor.a = Mathf.Lerp(startAlpha, endAlpha, _playerFadingTimer / _playerFadingTime);
            _playerMeshRenderer.material.color =  matColor;
            _playerFadingTime += Time.deltaTime;
            yield return Time.deltaTime;
        }

        _playerFadingTimer = 0;
        _isFading = false;
    }
}
