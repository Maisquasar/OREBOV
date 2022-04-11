using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public UnityEvent DetectedEvent;

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
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            DetectedEvent.Invoke();
        }
    }

    public static void LogDetected()
    {
        Debug.Log("Detected");
    }
}
