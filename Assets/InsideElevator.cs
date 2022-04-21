using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideElevator : MonoBehaviour
{
    Elevator _elevator;
    Transform _player;
    Transform _playerParent;

    // Start is called before the first frame update
    void Start()
    {
        _elevator = gameObject.transform.parent.GetComponent<Elevator>();
        _player = FindObjectOfType<PlayerStatus>().transform.parent;
        _playerParent = _player.transform.parent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            if (!_elevator.CoroutineEnd)
            {
                _player.transform.SetParent(_elevator.transform);
            }
            else
            {
                _player.transform.SetParent(_playerParent);
            }
        }
    }
}
