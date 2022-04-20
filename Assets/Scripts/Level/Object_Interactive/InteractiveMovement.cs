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


    private PlayerInteraction _playerInteraction;
    private Rigidbody _rigidbodyPlayer;

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
        
        _rigidbodyPlayer.isKinematic = true;
        _rigidbodyPlayer.useGravity = false;    
        _playerInteraction.LinkObject(this);
            
        _posStart = _startPoint.position;
        _posEnd = _endPoint.position;
    }


    protected void Update()
    {
        if (ObjectActive)
        {
            float ratio = (_movementTimer / _movementTime);
            _playerGO.transform.position = Vector3.Lerp(_posStart, _posEnd, ratio);
            _movementTimer += Time.deltaTime;

            if (ratio >= 1f)
            {
                if (!_endMovement) FinishVerticalMouvement();
                else CancelUpdate();
            }
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


    protected override void DeactiveItem()
    {
        ObjectActive = false;
        _rigidbodyPlayer.isKinematic = false;
        _rigidbodyPlayer.useGravity = true;
        _playerInteraction.UnlinkObject();
    }

}
