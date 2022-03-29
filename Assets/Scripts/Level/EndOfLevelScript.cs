using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Component t = other.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                ((PlayerController)t).GetComponentInChildren<PauseMenuScript>().OnEndLevel();
            }
        }
    }
}
