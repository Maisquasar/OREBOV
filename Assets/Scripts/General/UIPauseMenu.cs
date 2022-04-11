using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _UIGameMenu;
    [SerializeField]
    private GameObject _UIPauseMenu;
    [SerializeField]
    private GameObject _UIFadeScreen;
    private Image _FadeScreen;
    [SerializeField]
    private EventSystem _eventSystem;
    private bool _pauseMenuActive;

    public void Start()
    {
        _UIGameMenu.SetActive(true);
        _FadeScreen = _UIFadeScreen.GetComponent<Image>();
        StartCoroutine(ScreenfadeOut(1.0f, 1.0f));
    }

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
        _UIPauseMenu.transform.Find("Resume").GetComponent<Button>().Select();
    }

    private void CloseMenu()

    {
        _pauseMenuActive = false;
        _UIPauseMenu.SetActive(_pauseMenuActive);
        _eventSystem.SetSelectedGameObject(null);
    }

    public IEnumerator ScreenfadeIn(float fadeTime, float waitTime = 0.0f)
    {
        yield return new WaitForSeconds(waitTime);
        _UIFadeScreen.SetActive(true);
        float timer = 0;
        Color tmp;
        while (timer < fadeTime)
        {
            tmp = _FadeScreen.color;
            tmp.a = timer / fadeTime;
            timer += Time.deltaTime;
            _FadeScreen.color = tmp;
            yield return Time.deltaTime;
        }
        tmp = _FadeScreen.color;
        tmp.a = 1.0f;
        _FadeScreen.color = tmp;
        yield return null;
    }

    public IEnumerator ScreenfadeOut(float fadeTime, float waitTime = 0.0f)
    {
        yield return new WaitForSeconds(waitTime);
        float timer = fadeTime;
        while (timer > 0)
        {
            Color tmp = _FadeScreen.color;
            tmp.a = timer / fadeTime;
            timer -= Time.deltaTime;
            _FadeScreen.color = tmp;
            yield return Time.deltaTime;
        }
        _UIFadeScreen.SetActive(false);
        yield return null;
    }
}
