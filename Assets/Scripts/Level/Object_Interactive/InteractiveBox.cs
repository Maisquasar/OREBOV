using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveBox : InteractiveObject
{
    [Header("Box Setting")]
    [SerializeField]
    private LayerMask _collisionMask;
    [SerializeField]
    private float _speedBox = 2f;
    [SerializeField]
    private float _moveTime = 1f;
    private float _moveTimer = 0f;

    [SerializeField]
    private float _timeBetweenMove = 0.7f;

    [SerializeField]
    private bool _activeMouvement = false;
    private Rigidbody _rigidbodyPlayer;

    protected override void ActiveItem(GameObject player)
    {
        base.ActiveItem(player);
        transform.SetParent(player.transform);
        _playerGO.GetComponent<Player>().enabled = false;
        _rigidbodyPlayer = _playerGO.GetComponent<Rigidbody>();
        player.GetComponent<PlayerInteraction>().LinkObject(this);
    }

    protected override void DeactiveItem()
    {
        base.DeactiveItem();
        transform.SetParent(null);
        _playerGO.GetComponent<Player>().enabled = true;
        _playerGO.GetComponent<PlayerInteraction>().UnlinkObject();
    }

    public override void UpdateItem(Vector2 axis)
    {
        base.UpdateItem(axis);
        if (_objectActive)
        {

            if (!_activeMouvement && axis.normalized.x != 0)

                StartCoroutine(MoveBox(axis.normalized.x));
        }
    }


    private IEnumerator MoveBox(float dir)
    {
        _activeMouvement = true;
        Debug.Log(dir);
        Vector3 startPos = _playerGO.transform.position;
        Vector3 endPos = _playerGO.transform.position + _playerGO.transform.right * dir * _speedBox;


        while (_moveTimer < _moveTime)
        {
            _rigidbodyPlayer.position = Vector3.Lerp(startPos, endPos, _moveTimer / _moveTime);
            _moveTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }

        _moveTimer = 0f;
        while (_moveTimer < _timeBetweenMove)
        {

            _moveTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        _moveTimer = 0f;
        _activeMouvement = false;
        _rigidbodyPlayer.velocity = Vector3.zero;

    }
}
