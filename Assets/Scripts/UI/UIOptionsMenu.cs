using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOptionsMenu : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(name);
    }


    public IEnumerator Test(GameObject uiObject)
    {
        yield return new WaitForSeconds(0.01f);
        _eventSystem.SetSelectedGameObject(uiObject);
        Debug.Log(uiObject.name);
        yield return null;
    }
}
