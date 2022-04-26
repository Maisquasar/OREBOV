using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _UIGameMenu;
    [SerializeField] private GameObject _UIPauseMenu;
    [SerializeField] private GameObject _UIFadeScreen;
    [SerializeField] private EventSystem _eventSystem;

    private Image _FadeScreen;
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
        foreach (AmbientTypeHolder item in FindObjectsOfType<AmbientTypeHolder>(false))
        {
            item.enabled = false;
        }
        foreach (Rigidbody item in FindObjectsOfType<Rigidbody>(false))
        {
            if (item.gameObject.name == "Ball_L" || item.gameObject.name == "Ball_R") continue;
            item.isKinematic = true;
        }
        foreach (Animator item in FindObjectsOfType<Animator>(false))
        {
            if (item.transform.parent.name == "PauseMenu") continue;
            item.speed = 0;
        }
#if UNITY_EDITOR
        foreach (ObjectManager item in FindObjectsOfType<ObjectManager>(true))
        {
            print(GetPath(item.transform));
        }
#endif
    }

#if UNITY_EDITOR
    private string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }
#endif

    private void CloseMenu()
    {
        _pauseMenuActive = false;
        _UIPauseMenu.SetActive(_pauseMenuActive);
        _eventSystem.SetSelectedGameObject(null);
        foreach (AmbientTypeHolder item in FindObjectsOfType<AmbientTypeHolder>(false))
        {
            item.enabled = true;
        }
        foreach (Rigidbody item in FindObjectsOfType<Rigidbody>(false))
        {
            if (item.gameObject.name == "Ball_L" || item.gameObject.name == "Ball_R") continue;
            item.isKinematic = false;
        }
        foreach (Animator item in FindObjectsOfType<Animator>(false))
        {
            if (item.transform.parent.name == "PauseMenu") continue;
            item.speed = 1;
        }
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
