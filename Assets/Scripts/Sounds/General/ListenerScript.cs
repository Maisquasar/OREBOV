using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerScript : MonoBehaviour
{
    private Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = transform.parent.GetComponentInChildren<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.position;
    }
}
