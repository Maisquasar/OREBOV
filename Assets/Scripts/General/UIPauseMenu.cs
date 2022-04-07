using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _UIPauseMenu;
    [SerializeField]
    private EventSystem _eventSystem;
    private bool _pauseMenuActive;

    public void CallPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
            ActiveMenu();
    }

   public void ActiveMenu()
    {
        if (_pauseMenuActive)
        {
            CloseMenu();
            return;
        }
        if (!_pauseMenuActive)
        {
            OpenMenu();
            return;
        }
    }

    private void OpenMenu()
    {
        _pauseMenuActive = true;
        _UIPauseMenu.SetActive(_pauseMenuActive);
        _UIPauseMenu.transform.GetChild(0).Find("Resume").GetComponent<Button>().Select();  
    }

    private void CloseMenu()

    {
        _pauseMenuActive = false;
        _UIPauseMenu.SetActive(_pauseMenuActive);
        _eventSystem.SetSelectedGameObject(null);
    }
}
