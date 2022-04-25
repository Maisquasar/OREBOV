using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileLight : MonoBehaviour
{
    [SerializeField] float Timer;
    [SerializeField] float _waitingTime = 2f;


    MobilePoint _mobilePoint;
    bool atDestination = false;
    Vector3 Goto;
    Vector3 _initialPos;



    private void OnDrawGizmos()
    {
        _mobilePoint = GetComponentInChildren<MobilePoint>();
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_mobilePoint.transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, _mobilePoint.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        _mobilePoint = GetComponentInChildren<MobilePoint>();
        _initialPos = transform.position;
        Goto = _mobilePoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!atDestination)
        {
            if (!_startCoroutine)
                StartCoroutine(WaitForSeconds(_waitingTime));
            else if (_coroutineEnd && _startCoroutine)
                Lerp(_initialPos, Goto, Timer);
        }
        else if (_coroutineEnd)
        {
            if (!_startCoroutine)
                StartCoroutine(WaitForSeconds(_mobilePoint.Time));
            else if (_coroutineEnd && _startCoroutine)
                Lerp(Goto, _initialPos, Timer);
        }
    }

    float _actualTime = 0;
    private void Lerp(Vector3 initial, Vector3 goTo, float duration)
    {
        if (_actualTime < duration)
        {
            transform.position = Vector3.Lerp(initial, goTo, _actualTime / duration);
            _actualTime += Time.deltaTime;
        }
        else
        {
            transform.position = goTo;
            _actualTime = 0;
            atDestination = !atDestination;
            _startCoroutine = false;
        }
    }

    bool _coroutineEnd = true;
    bool _startCoroutine = false;
    IEnumerator WaitForSeconds(float time)
    {
        _startCoroutine = true;
        _coroutineEnd = false;
        yield return new WaitForSeconds(time);
        _coroutineEnd = true;
    }
}
