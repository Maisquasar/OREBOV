using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : Trigger
{
    [Header("Scenes Trigger Settings")]
    [SerializeField] private int _sceneIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            SceneManager.LoadScene(_sceneIndex);
        }
    }
}
