using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _UIPauseMenu;
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
    }

    private void CloseMenu()

    {
        _pauseMenuActive = false;
        _UIPauseMenu.SetActive(_pauseMenuActive);
    }
}
