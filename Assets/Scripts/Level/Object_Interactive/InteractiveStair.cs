using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveStair : InteractiveObject
{

    [SerializeField, Header("Stair Settings")]
    private Vector3[] _stairPoints = new Vector3[4];
    [SerializeField] private float _interactionTime;


    private float _mouvementTimer;
    private float _distanceInteraction;
    private bool _isStairUp = true;


    private PlayerInteraction _playerInteraction;
    private Rigidbody _rigidbodyPlayer;
    private PlayerStatus _playerStatus;

    protected override void Start()
    {
        base.Start();
        ObjectType = InteractObject.InteractObjects.Stair;
    }

    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        InitComponents();
        InitInteraction();
        StartCoroutine(StairMouvement());
    }

    #region Activate Item Functions 
    private void InitComponents()
    {
        _rigidbodyPlayer = _playerGO.GetComponent<Rigidbody>();
        _playerInteraction = _playerGO.GetComponent<PlayerInteraction>();
        _playerStatus = _playerGO.GetComponent<PlayerStatus>();
    }

    private void InitInteraction()
    {
        _rigidbodyPlayer.isKinematic = true;
        _playerInteraction.LinkObject(this);
        _isStairUp = _stairPoints[0].y < _stairPoints[3].y;
    }

    #endregion


    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        _rigidbodyPlayer.isKinematic = false;
        _playerInteraction.UnlinkObject();
        _playerStatus.Controller.ActiveStair(false, false);
        _playerStatus.Controller.ActiveStair(false, true);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Color _posColor = Color.green;
        for (int i = 0; i < _stairPoints.Length; i++)
        {
            float index = i;
            _posColor = Color.Lerp(Color.red, Color.green, (index / _stairPoints.Length));
            Gizmos.color = _posColor;
            Gizmos.DrawSphere(transform.position + _stairPoints[i], 0.125f);
        }
    }

    private IEnumerator StairMouvement()
    {
        StartInteraction();
        yield return StartCoroutine(PointToPointMovement(_stairPoints[0], _stairPoints[1]));
        MiddleInteraction();
        yield return StartCoroutine(PointToPointMovement(_stairPoints[2], _stairPoints[3])); ;
        EndInteraction();
    }

    private IEnumerator PointToPointMovement(Vector3 p1, Vector3 p2)
    {
        _mouvementTimer = 0f;
        while (_mouvementTimer < _interactionTime / 2f)
        {
            _playerGO.transform.position = Vector3.Lerp(transform.position + p1, transform.position + p2, _mouvementTimer / (_interactionTime / 2f));
            _mouvementTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
    }

    private void StartInteraction()
    {
        _playerGO.transform.rotation = Quaternion.Euler(0, 0f, 0);
        _playerStatus.Controller.ActiveStair(true, _isStairUp);
    }



    private void MiddleInteraction()
    {
        _playerGO.transform.rotation = Quaternion.Euler(0, 180f, 0);
        _playerGO.transform.position = transform.position + _stairPoints[2];
    }

    private void EndInteraction()
    {
        _playerGO.transform.rotation = Quaternion.Euler(0, 90f, 0);
        _rigidbodyPlayer.isKinematic = false;
        _playerStatus.Controller.ActiveStair(false, false);
        _playerStatus.Controller.ActiveStair(false, true);
        _playerInteraction.UnlinkObject();
    }

}
