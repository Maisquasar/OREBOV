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

    [SerializeField] float _exitDirection = 1;
    [SerializeField] bool IsDown;
    [SerializeField] bool LockPosition = true;
    [SerializeField] float ExitDistance;

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
        _playerInteraction.LinkObject(this);
        _climbLadder.PlaySound();

        _posStart = _startPoint.position;
        _posEnd = _endPoint.position;
    }

    private void OnDrawGizmos()
    {
        if (!LockPosition)
        {
            transform.position = new Vector3(_startPoint.position.x, transform.position.y, transform.position.z);
            return;
        }
        _playerStatus = FindObjectOfType<PlayerStatus>();
        transform.position = new Vector3(transform.position.x, transform.position.y, _playerStatus.transform.position.z);
    }


    protected void Update()
    {
        if (ObjectActive && !climb)
        {
            StartCoroutine(LerpFromTo(_posStart, _posEnd + Vector3.down * 0.4f, _movementTime));
        }
    }

    private void FinishVerticalMouvement()
    {
        /*
        _endMovement = true;
        _movementTimer = 0f;
        Vector3 dir = (_endPoint.position - _startPoint.position).normalized;
        dir.y = 0;
        _posStart = _endPoint.position;
        _posEnd = _endPoint.position + dir.normalized * 1f;
        */
        if (IsDown)
            StartCoroutine(_playerStatus.Controller.LerpFromTo(_playerStatus.transform.position, _playerStatus.transform.position + Vector3.forward * ExitDistance + Vector3.up * 1f, 0.2f));
    }

    bool climb = false;
    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        climb = true;

        //Flip player
        FlipEnterPlayer();

        _playerStatus.Controller.Climb(true, initial.y < goTo.y ? 1 : -1);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            if (_playerStatus.transform.eulerAngles.y != transform.eulerAngles.y)
                FlipEnterPlayer();
            _playerGO.transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        _playerGO.transform.position = goTo;

        climb = false;

        _playerStatus.Controller.Climb(false);

        FinishVerticalMouvement();

        DeactiveItem();
        //Flip player
        FlipExitPlayer();


    }

    protected void FlipEnterPlayer()
    {
        _playerStatus.Controller.canTurn = false;
        var tmpRot = _playerStatus.transform.eulerAngles;
        tmpRot.y = transform.eulerAngles.y;
        _playerStatus.transform.eulerAngles = tmpRot;
    }


    protected void FlipExitPlayer()
    {
        _playerStatus.Controller.canTurn = true;
        var tmpRot = _playerStatus.transform.eulerAngles;
        _playerStatus.Controller.Direction = _exitDirection;
        tmpRot.y = _exitDirection * 90;
        _playerStatus.transform.eulerAngles = tmpRot;
    }

    protected override void DeactiveItem()
    {
        ObjectActive = false;
        _rigidbodyPlayer.isKinematic = false;
        _playerInteraction.UnlinkObject();
        _climbLadder.StopSound();
    }

}
