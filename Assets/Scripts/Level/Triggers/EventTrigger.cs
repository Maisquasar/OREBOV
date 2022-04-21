using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : Trigger
{
    [SerializeField] private UnityEvent _event;

    [Header("Trigger Setting")]
    [SerializeField] private bool _playOnce = true;
    private void OnTriggerEnter(Collider other)
    {
        //TODO : add enemies
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            _event.Invoke();
            if (_playOnce) Destroy(gameObject);
        }
    }
}

