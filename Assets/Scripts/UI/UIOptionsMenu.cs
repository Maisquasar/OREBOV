using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOptionsMenu : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _fullScreenButton;
    [SerializeField] private GameObject _vSyncButton;

    private void Start()
    {
        _fullScreenButton.SetActive(Screen.fullScreen);
        _vSyncButton.SetActive(QualitySettings.vSyncCount != 0);
    }

    public IEnumerator SetSelectedObject(GameObject uiObject)
    {
        yield return new WaitForSeconds(0.01f);
        _eventSystem.SetSelectedGameObject(uiObject);
        yield return null;
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetVSync()
    {
        if (QualitySettings.vSyncCount == 0)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
