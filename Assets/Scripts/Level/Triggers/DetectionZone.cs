using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public UnityEvent DetectedEvent;
    public StaticEnemy Enemy;
    [HideInInspector] public bool Detect;

    private void OnDrawGizmos()
    {
        transform.position = Enemy.transform.position;
    }

    public override void Start()
    {
        base.Start();
        if (DetectedEvent == null)
        {
            DetectedEvent.AddListener(LogDetected);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            DetectedEvent.Invoke();
            Detect = true;
        }
        else
            Detect = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            Detect = false;
        }
    }

    public static void LogDetected()
    {
        Debug.Log("Detected");
    }
}
