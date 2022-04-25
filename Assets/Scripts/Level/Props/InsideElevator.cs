using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideElevator : MonoBehaviour
{
    Elevator _elevator;

    Dictionary<Collider, Transform> _parents = new Dictionary<Collider, Transform>();

    void Start()
    {
        _elevator = gameObject.transform.parent.GetComponent<Elevator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.GetComponent<Rigidbody>() || other.gameObject.GetComponent<EntityWalkSounds>())
            return;
        bool alreadyIn = false;
        foreach (var parent in _parents)
        {
            if (parent.Key == other)
            {
                alreadyIn = true;
            }
        }
        if (!alreadyIn)
            _parents.Add(other, other.gameObject.transform.parent);
        if (!_elevator.CoroutineEnd)
        {
            other.gameObject.transform.SetParent(_elevator.transform);
        }
        else
        {
            other.gameObject.transform.SetParent(_parents[other]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_parents[other] != null)
            other.gameObject.transform.SetParent(_parents[other]);
    }
}
