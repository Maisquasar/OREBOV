using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOptionsMenu : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;

    public IEnumerator SetSelectedObject(GameObject uiObject)
    {
        yield return new WaitForSeconds(0.01f);
        _eventSystem.SetSelectedGameObject(uiObject);
        Debug.Log(uiObject.name);
        yield return null;
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        
    }
}
