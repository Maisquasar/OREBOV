using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _UIMainMenu;
    [SerializeField] private GameObject _UILevelMenu;
    [SerializeField] private GameObject _UIOptionMenu;
    [SerializeField] private GameObject _UIFadeScreen;
    [SerializeField] private EventSystem _eventSystem;

    private Image _FadeScreen;

    public void Start()
    {
        _UIMainMenu.SetActive(true);
        _FadeScreen = _UIFadeScreen.GetComponent<Image>();
        StartCoroutine(ScreenfadeOut(1.0f, 1.0f));
    }

    public void ActiveMainMenu()
    {
        _UIMainMenu.SetActive(true);
        _UILevelMenu.SetActive(false);
        _UIOptionMenu.SetActive(false);
        _UIMainMenu.transform.Find("Play").GetComponent<Button>().Select();
    }

    public void ActiveOptionMenu()
    {
        _UIMainMenu.SetActive(false);
        _UILevelMenu.SetActive(false);
        _UIOptionMenu.SetActive(true);
        _UIOptionMenu.transform.Find("Back").GetComponent<Button>().Select();
    }

    public void ActiveLevelMenu()
    {
        _UIMainMenu.SetActive(false);
        _UILevelMenu.SetActive(true);
        _UIOptionMenu.SetActive(false);
        _UILevelMenu.transform.Find("Levels").Find("Level1").GetComponent<Button>().Select();
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
