using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Debug
public class CameraSettings : MonoBehaviour
{
    [SerializeField] public bool FollowPlayer;
    [SerializeField] private float _ratio;
    PlayerStatus _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowPlayer)
            transform.position = new Vector3(_player.transform.position.x + 1 * _ratio, _player.transform.position.y * _ratio + 4, transform.position.z);
    }
}
