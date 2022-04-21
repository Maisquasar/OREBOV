using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField]
    private bool _doorState;
    [SerializeField]
    private float _movementDuration;


    [SerializeField]
    private Vector3 _pivotPoint;
    [SerializeField]
    private Vector3 _endPosition;



    private Vector3 _startPosition;
    private float _movementTimer;
    private IEnumerator coroutine;

    private void Start()
    {
        _startPosition = transform.position;
        _endPosition += _startPosition;
        coroutine = DoorMovement();
    }

    public void ActiveDoor()
    {
        _doorState = !_doorState;
        StopCoroutine(coroutine);
        coroutine = DoorMovement();
        StartCoroutine(coroutine);
    }

    private IEnumerator DoorMovement()
    {
        Vector3 startPos = _doorState == true ? _startPosition : _endPosition;
        Vector3 endPos = _doorState == false ? _startPosition : _endPosition;
        while (_movementTimer < _movementDuration)
        {
            float ratio = _movementTimer / _movementDuration;
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            _movementTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        _movementTimer = 0;

    }

    private void OnDrawGizmos()
    {
        if (Vector3.zero == _startPosition)
        {
            Vector3 endPos = transform.position + _endPosition;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPos, 0.125f);
        }
    }
}
