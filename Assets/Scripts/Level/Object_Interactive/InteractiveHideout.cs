using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveHideout : InteractiveObject
{
    [Header("Hideout Setting")]
    [SerializeField] private float _playerFadingTime;
    [SerializeField] private float _playerFadingTimer;
    [SerializeField] private bool _isFading;

    private IEnumerator _startCoroutine;
    private IEnumerator _endCoroutine;

    private PlayerStatus _playerStatus;
    private PlayerInteraction _playerInteraction;
    private SkinnedMeshRenderer _playerMeshRenderer;


    protected override void Start()
    {
        base.Start();
        ObjectType = InteractObject.InteractObjects.Hideout;
        _playerStatus = FindObjectOfType<PlayerStatus>();
    }

    protected override void ActiveItem(GameObject player)
    {

        if (!ObjectActive)
        {
            base.ActiveItem(player);

            // Get the component for the first time
            GetPlayerComponent();

            // Cancel player fading coroutine if it already running
            if (_isFading) StopCoroutine(_endCoroutine);

            ObjectActive = true;
            _playerStatus.Hide(true, transform.position);
            _playerInteraction.LinkObject(this);
            _startCoroutine = PlayerFading(true);

            StartCoroutine(_startCoroutine);
        }
    }

    public override void ItemInteraction(GameObject player)
    {
        if (_playerStatus.MoveDir.x == 0 && _playerStatus.Controller.CanHide) 
            ActiveItem(player);
    }

    public override void UpdateItem(Vector2 axis)
    {
        base.UpdateItem(axis);
        if (axis.x != 0) DeactiveItem();
    }

    protected override void DeactiveItem()
    {
        if (ObjectActive)
        {
            base.DeactiveItem();

            // Cancel player fading if it already running
            if (_isFading) StopCoroutine(_startCoroutine);

            _endCoroutine = PlayerFading(false);
            _playerInteraction.UnlinkObject();
            _playerStatus.Hide(false, transform.position);
            ObjectActive = false;

            StartCoroutine(_endCoroutine);
        }
    }

    private void GetPlayerComponent()
    {
        if (_playerStatus == null) _playerStatus = _playerGO.GetComponent<PlayerStatus>();
        if (_playerInteraction == null) _playerInteraction = _playerGO.GetComponent<PlayerInteraction>();
        if (_playerMeshRenderer == null) _playerMeshRenderer = _playerGO.GetComponentInChildren<SkinnedMeshRenderer>();
    }


    // Player Coroutine Fading
    private IEnumerator PlayerFading(bool active)
    {
        Color matColor = _playerMeshRenderer.material.color;
        float startAlpha = matColor.a;
        float endAlpha = active == false ? 1f : 0f;

        _isFading = true;
        _playerFadingTimer = 0;

        while (_playerFadingTimer < _playerFadingTime)
        {

            matColor.a = Mathf.Lerp(startAlpha, endAlpha, _playerFadingTimer / _playerFadingTime);
            _playerMeshRenderer.materials[0].color = matColor;
            _playerFadingTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }


        _isFading = false;
    }



}
