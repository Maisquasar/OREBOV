using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Debug
public class CameraSettings : MonoBehaviour
{
    [SerializeField] public bool FollowPlayer;
    [SerializeField] float ratio;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowPlayer)
            transform.position = new Vector3(player.transform.position.x + 1 * ratio, player.transform.position.y * ratio + 4, transform.position.z);
    }
}
