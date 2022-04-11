using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;


public class InteractiveBox : InteractiveObject
{
    [Header("Box Setting")]
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _speedBox = 2f;
    [SerializeField] private float _moveTime = 1f;
    [SerializeField] private bool _activeMouvement = false;

    private Rigidbody _rigidbodyPlayer;
    private float _moveTimer = 0f;
    private float _timeBetweenMove = 0.7f;

    [Header("Box Debug")]
    [SerializeField] private bool _activeBoxDebug;
    [SerializeField] private int mouvementCount = 1;
    private PlayerInteraction PlayerInteract;
    private Player _playerStatus;

    private void Start()
    {
        ObjectType = "Box";
    }

    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        transform.SetParent(player.transform);
        PlayerInteract = _playerGO.GetComponent<PlayerInteraction>();
        _playerStatus = _playerGO.GetComponent<Player>();
        PlayerInteract.LinkObject(this);
        _rigidbodyPlayer = _playerGO.GetComponent<Rigidbody>();
        _activeMouvement = true;
        StartCoroutine(PauseBoxMouvement());
    }

    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        transform.SetParent(null);
        _playerGO.GetComponent<PlayerInteraction>().UnlinkObject();
    }

    public override void UpdateItem(Vector2 axis)
    {
        base.UpdateItem(axis);
        if (_objectActive)
        {

            Debug.DrawRay(transform.position + new Vector3(1, 0, 0) * transform.localScale.x / 2f, new Vector3(1, 0, 0) * _speedBox, Color.green);
            if (_useOnlyInShadow )
            {

                if (!_activeMouvement && axis.normalized.x != 0 && _playerStatus.IsShadow)
                {
                    StartCoroutine(MoveBox(axis.normalized.x));
                    _playerStatus.PlayRightAnimation(axis.x);
                }
            }else
            {
                if (!_activeMouvement && axis.normalized.x != 0)
                {
                    StartCoroutine(MoveBox(axis.normalized.x));
                    _playerStatus.PlayRightAnimation(axis.x);

                }
            }
        }
    }

    private void ShowBoxMouvement()
    {
        Vector3 startPos = transform.position + new Vector3(1, 0, 0) * transform.localScale.x / 2f;

        for (int i = 0; i < mouvementCount; i++)
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

    private void OnDrawGizmosSelected()
    {
        if (_activeBoxDebug) ShowBoxMouvement();
    }

    private IEnumerator MoveBox(float dir)
    {
        _activeMouvement = true;
        PlayerInteract.CanStopNow = false;
        Vector3 startPos = _playerGO.transform.position;
        Vector3 endPos = _playerGO.transform.position + Vector3.right * dir * _speedBox;
        Debug.DrawRay(transform.position + new Vector3(dir, 0, 0) * transform.localScale.x, new Vector3(dir, 0, 0) * _speedBox, Color.green);
        if (Physics.Raycast(transform.position + new Vector3(dir, 0, 0) * transform.localScale.x / 2f, new Vector3(dir, 0, 0), _speedBox, _collisionMask, QueryTriggerInteraction.Ignore))
        {
            StartCoroutine(PauseBoxMouvement());
            yield break;
        }

        while (_moveTimer < _moveTime)
        {
            _rigidbodyPlayer.position = Vector3.Lerp(startPos, endPos, _moveTimer / _moveTime);
            _moveTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        _moveTimer = 0f;
        _rigidbodyPlayer.velocity = Vector3.zero;
        StartCoroutine(PauseBoxMouvement());
        DeactiveItem();
    }

    private IEnumerator PauseBoxMouvement()
    {
        PlayerInteract.CanStopNow = true;
        while (_moveTimer < _timeBetweenMove)
        {

            _moveTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        _activeMouvement = false;
        _moveTimer = 0f;
    }
}
