using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using InteractObject;

public class InteractiveBox : InteractiveObject
{
    [Header("Box Setting")]
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _speedBox = 2f;
    private bool _startMovement = false;

    [Header("Sounds")]
    [SerializeField] private SoundEffectsHandler _boxPushInt;
    [SerializeField] private SoundEffectsHandler _boxPushExt;
    [SerializeField] private SoundEffectsHandler _boxGrab;
    [SerializeField] private SoundEffectsHandler _boxRelease;

    [Header("Box Debug")]
    [SerializeField] private bool _activeBoxDebug;
    [SerializeField] private int _mouvementCount = 1;
    
    private Rigidbody _rigidbodyPlayer;
    private PlayerInteraction _playerInteract;
    private PlayerStatus _playerStatus;
    private PlayerAnimator _playerAnimator;

    private Vector3 delta;
    private bool _hasMove;
    private Vector3 _localScale;

    override protected void Start()
    {
        base.Start();
        _localScale = transform.localScale;
        ObjectType = InteractObjects.Box;
    }

    protected override void ActiveItem(GameObject player)
    {
        if (!ObjectActive)
        {
            base.ActiveItem(player);

            ObjectActive = true;
            _startMovement = false;
            delta = transform.position - _playerGO.transform.position;
            _hasMove = false;

            GetPlayerComponent();

            _playerAnimator.SetPush(false);
            _playerInteract.LinkObject(this);
            _boxGrab.PlaySound();
        }
    }

    private void GetPlayerComponent()
    {
        if (_playerInteract == null)
        {
            _playerInteract = _playerGO.GetComponent<PlayerInteraction>();
            _playerStatus = _playerGO.GetComponent<PlayerStatus>();
            _playerAnimator = _playerGO.GetComponent<PlayerAnimator>();
            _rigidbodyPlayer = _playerGO.GetComponent<Rigidbody>();
        }
    }

    protected override void DeactiveItem()
    {

        if (ObjectActive)
        {
            ObjectActive = false;
            _playerStatus.PlayerActionState = States.PlayerAction.IDLE;
            _playerInteract.CanStopNow = true;
            _startMovement = false;

            DisableAnimations();
            base.DeactiveItem();
            _playerInteract.UnlinkObject();
            _playerAnimator.SetPush(true);

            _boxPushInt.StopSound();
            _boxPushExt.StopSound();
            _boxRelease.PlaySound();
        }

    }

    private void DisableAnimations()
    {
        _playerStatus.Controller.Pull(false);
        _playerStatus.Controller.Push(false);
    }

    public override void UpdateItem(Vector2 axis)
    {
        base.UpdateItem(axis);

        if (axis.x == 0)
        {
            DisableAnimations();
        }

        Debug.DrawRay(transform.position + new Vector3(1, 0, 0) * transform.localScale.x / 2f, new Vector3(1, 0, 0) * _speedBox, Color.green);
        if (axis.normalized.x != 0 && (!UseOnlyInShadow || _playerStatus.IsShadow))
        {
            MovingBox(_axis.x);
            _playerStatus.PlayRightAnimation(axis.x);
        }
        else if (axis.normalized.x == 0 && (!UseOnlyInShadow || _playerStatus.IsShadow))
        {
            if (_hasMove) DeactiveItem();
          
        }
    }

    private void MovingBox(float dir)
    {
        StartMouvement();
        _hasMove = false;
        _playerInteract.CanStopNow = false;
        if (CanBoxMove(dir))
        {
            _rigidbodyPlayer.position += Vector3.right * dir * _speedBox * Time.deltaTime;
            transform.position = _rigidbodyPlayer.position + delta;
        }
    }


    private bool CanBoxMove(float dir)
    {
        return !Physics.Raycast(transform.position + new Vector3(dir, 0, 0) * transform.localScale.x / 2f, new Vector3(dir, 0, 0), _speedBox * Time.deltaTime, _collisionMask, QueryTriggerInteraction.Ignore);
    }

    private void StartMouvement()
    {
        if (!_startMovement)
        {
            _startMovement = true;
            if (AmbientType == AmbientSoundType.Interior)
            {
                _boxPushInt.PlaySound();
            }
            else
            {
                _boxPushExt.PlaySound();
            }
        }
    }


    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (_activeBoxDebug) ShowBoxMouvement();
    }

    private void ShowBoxMouvement()
    {
        Vector3 startPos = transform.position + new Vector3(1, 0, 0) * transform.localScale.x / 2f;

        for (int i = 0; i < _mouvementCount; i++)
        {

            Gizmos.color = Color.green;
            if (CheckObstacle(startPos))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(startPos, startPos + new Vector3(1, 0, 0) * _speedBox);
                startPos = startPos + new Vector3(1, 0, 0) * _speedBox;
                Gizmos.DrawSphere(startPos, 0.25f);
                break;
            }
            Gizmos.DrawLine(startPos, startPos + new Vector3(1, 0, 0) * _speedBox);

            startPos = startPos + new Vector3(1, 0, 0) * _speedBox;
            Gizmos.DrawSphere(startPos, 0.25f);
        }
    }
    private bool CheckObstacle(Vector3 startPos)
    {
        bool frontTest = Physics.Raycast(startPos, new Vector3(1, 0, 0), _speedBox, _collisionMask, QueryTriggerInteraction.Ignore);
        if (frontTest) return true;
        bool backTest = Physics.Raycast(startPos + new Vector3(1, 0, 0) * _speedBox, new Vector3(0, -1, 0), _speedBox, _groundMask, QueryTriggerInteraction.Ignore);
        if (!backTest)
            return true;
        else
            return false;

    }
    #endregion

}
