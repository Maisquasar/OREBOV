using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CookSomePopCorn(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            print("Pop !");
        }
        print("Cooked " + quantity + " popcorn !");
    }
}
