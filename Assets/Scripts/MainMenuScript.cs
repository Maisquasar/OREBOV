using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Button m_buttonStart, m_buttonLevels, m_buttonQuit;
    public GameObject MainMenuUI;
    public GameObject LevelsMenuUI;
    public GameObject FadeScreen;
    public Image FadeColor;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        if (MainMenuUI == null) MainMenuUI = new GameObject();
        if (LevelsMenuUI == null) MainMenuUI = new GameObject();
        if (FadeScreen == null) FadeScreen = new GameObject();
        if (FadeColor == null) FadeColor = gameObject.AddComponent<Image>();
        FadeScreen.SetActive(true);
        StartCoroutine(ScreenFadeOut());
    }

    public void OnClickStart()
    {
        StartCoroutine(StartScript());
    }

    public void OnClickStart2()
    {
        StartCoroutine(StartScript2());
    }

    public void OnClickLevels()
    {
        LevelsMenuUI.SetActive(true);
        MainMenuUI.SetActive(false);
    }

    public void OnClickQuit()
    {
        Application.Quit(0);
    }

    public void OnClickBack()
    {
        MainMenuUI.SetActive(true);
        LevelsMenuUI.SetActive(false);
    }

    IEnumerator StartScript()
    {
        yield return ScreenFadeIn();
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        yield return null;
    }

    IEnumerator StartScript2()
    {
        yield return ScreenFadeIn();
        SceneManager.LoadScene("Level2", LoadSceneMode.Single);
        yield return null;
    }

    public IEnumerator ScreenFadeOut()
    {
        float timer = 1.0f;
        while (timer > 0)
        {
            Color tmp = FadeColor.color;
            tmp.a = timer;
            FadeColor.color = tmp;
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        FadeScreen.SetActive(false);
    }

    public IEnumerator ScreenFadeIn()
    {
        FadeScreen.SetActive(true);
        float timer = 0.0f;
        while (timer < 1)
        {
            Color tmp = FadeColor.color;
            tmp.a = timer;
            FadeColor.color = tmp;
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }
    }
}
