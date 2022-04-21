using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveMovement : InteractiveObject
{
    [Header("Movement Setting")]
    [SerializeField]
    protected Transform _startPoint;
    [SerializeField]
    protected Transform _endPoint;
    [SerializeField]
    private float _movementTime = 1f;
    [SerializeField]
    protected float _movementTimer = 0f;
    [SerializeField]
    SoundEffectsHandler _climbLadder;


    private PlayerInteraction _playerInteraction;
    private Rigidbody _rigidbodyPlayer;
    private PlayerStatus _playerStatus;

    private Vector3 _posEnd;
    private Vector3 _posStart;
    private bool _endMovement;

    // TODO Clean the code

    protected override void Start()
    {
        base.Start();
        ObjectType = InteractObject.InteractObjects.Ladder;
    }

    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        _movementTimer = 0;
        _rigidbodyPlayer = _playerGO.GetComponent<Rigidbody>();
        _playerInteraction = _playerGO.GetComponent<PlayerInteraction>();
        _playerStatus = _playerGO.GetComponent<PlayerStatus>();

        _rigidbodyPlayer.isKinematic = true;
        _rigidbodyPlayer.useGravity = false;
        _playerInteraction.LinkObject(this);
        _climbLadder.PlaySound();

        _posStart = _startPoint.position;
        _posEnd = _endPoint.position;
    }


    protected void Update()
    {
        if (ObjectActive  && !climb)
        {
            StartCoroutine(LerpFromTo(_posStart + Vector3.left * 0.2f * _playerStatus.Controller.Direction, _posEnd + Vector3.left * 0.2f * _playerStatus.Controller.Direction + Vector3.down * 0.4f, _movementTime));
        }
    }

    private void FinishVerticalMouvement()
    {
        _endMovement = true;
        _movementTimer = 0f;
        Vector3 dir = (_endPoint.position - _startPoint.position).normalized;
        dir.y = 0;
        _posStart = _endPoint.position;
        _posEnd = _endPoint.position + dir.normalized * 1f;
    }

    bool climb = false;
    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        climb = true;
        _playerStatus.Controller.Climb(true, initial.y < goTo.y ? 1 : -1);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            _playerGO.transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }

        _playerGO.transform.position = goTo;
        climb = false;
        _playerStatus.Controller.Climb(false);
        FinishVerticalMouvement();
        DeactiveItem();
    }

    protected override void DeactiveItem()
    {
        ObjectActive = false;
        _rigidbodyPlayer.isKinematic = false;
        _playerInteraction.UnlinkObject();
    }

}
