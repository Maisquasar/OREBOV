using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileLight : MonoBehaviour
{
    [SerializeField] float Timer;


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
        if (!atDestination && CoroutineEnd)
        {
            StartCoroutine(LerpFromTo(transform.position, Goto, Timer));
        }
        else if (CoroutineEnd)
        {
            StartCoroutine(LerpFromTo(transform.position, _initialPos, Timer));
        }
    }

    bool CoroutineEnd = true;
    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        CoroutineEnd = false;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.position = goTo;
        CoroutineEnd = true;
        atDestination = !atDestination;
    }
}
