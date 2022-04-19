using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : SwitchableObjects
{
    MobilePoint _mobilePoint;

    [SerializeField] float Timer;

    BelowElevatorDetector _detector;

    Vector3 _initialPos;
    Vector3 Goto;
    // Start is called before the first frame update
    void Start()
    {
        _mobilePoint = GetComponentInChildren<MobilePoint>();
        _detector = GetComponentInChildren<BelowElevatorDetector>();
        _initialPos = transform.position;
        Goto = _mobilePoint.transform.position;
    }

    private void OnDrawGizmos()
    {
        _mobilePoint = GetComponentInChildren<MobilePoint>();
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(_mobilePoint.transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, _mobilePoint.transform.position);
    }

    private void Update()
    {
        if (_detector.Detect)
        {
            cancel = true;
            StartCoroutine(WaitForRestart());
        }
    }

    bool atDestination = false;
    public override void Activate()
    {
        if (!atDestination && CoroutineEnd)
        {
            StartCoroutine(LerpFromTo(transform.position, Goto, Timer));
        }
        else if (CoroutineEnd)
        {
            StartCoroutine(LerpFromTo(transform.position, _initialPos, Timer));
        }
    }
    bool cancel = false;

    bool CoroutineEnd = true;
    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        CoroutineEnd = false;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(initial, goTo, t / duration);
            if (cancel)
            {
                cancel = false;
                yield break;
            }
            yield return 0;
        }
        transform.position = goTo;
        CoroutineEnd = true;
        atDestination = !atDestination;
    }

    IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(1);
        if (!atDestination)
        {
            var Distance = Vector3.Distance(transform.position, _initialPos);
            var Distance2 = Vector3.Distance(Goto, _initialPos);
            float tmp = (Distance * Timer) / Distance2;
            StartCoroutine(LerpFromTo(transform.position, _initialPos, tmp));
        }
        else
        {
            var Distance = Vector3.Distance(transform.position, Goto);
            var Distance2 = Vector3.Distance(Goto, _initialPos);
            float tmp = (Distance * Timer) / Distance2;
            Debug.Log(tmp);
            StartCoroutine(LerpFromTo(transform.position, Goto, tmp));
        }
    }
}
