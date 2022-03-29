using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PauseMenuScript : MonoBehaviour
{
    public Button m_buttonContinue, m_buttonReset, m_buttonQuit;
    public TextMeshProUGUI CointRenderer;
    public TextMeshProUGUI LivesRenderer;
    public PlayerController Controller;
    public GameObject PauseMenuUI;
    public GameObject GameplayUI;
    public GameObject GameOverUI;
    public GameObject EOLUI;
    public GameObject FadeScreen;
    public Image FadeColor;
    public UnityEvent ResetEvent;
    public GameObject HealthIcon;
    public GameObject HealthContainer;
    public Color HealthColor;

    public Camera MainCam;
    public Camera FPSCam;

    List<GameObject> healthObjects;
    int coinCount = 0;
    float GlobalTimeScale;
    // Start is called before the first frame update
    void Start()
    {
        if (Controller == null) Controller = new PlayerController();
        if (PauseMenuUI == null) PauseMenuUI = new GameObject();
        if (GameplayUI == null) GameplayUI = new GameObject();
        if (FadeScreen == null) FadeScreen = new GameObject();
        if (FadeColor == null) FadeColor = gameObject.AddComponent<Image>();
        if (ResetEvent == null) ResetEvent = new UnityEvent();
        if (CointRenderer == null) CointRenderer = new TextMeshProUGUI();
        healthObjects = new List<GameObject>();
        GlobalTimeScale = Time.timeScale;
        OnHealthChange();
        OnLivesChange();
        FadeScreen.SetActive(true);
        StartCoroutine(ScreenFadeOut());
    }

    public void OnClickContinue()
    {
        Controller.enabled = true;
        PauseMenuUI.SetActive(false);
        Time.timeScale = GlobalTimeScale;
    }

    public void OnViewChange()
    {
        RenderTexture tmp = MainCam.targetTexture;
        MainCam.targetTexture = FPSCam.targetTexture;
        FPSCam.targetTexture = tmp;
    }
    public void OnPause()
    {
        Controller.enabled = PauseMenuUI.activeInHierarchy;
        PauseMenuUI.SetActive(!Controller.enabled);
        if (Controller.enabled)
        {
            Time.timeScale = GlobalTimeScale;
        }
        else
        {
            Time.timeScale = 0.0f;
        }
    }

    public void OnGameOver()
    {
        Controller.enabled = false;
        GameOverUI.SetActive(true);
    }

    public void OnEndLevel()
    {
        StartCoroutine(EndLevel());
    }

    public void OnClickReset()
    {
        ResetEvent.Invoke();
        Controller.enabled = true;
        Time.timeScale = GlobalTimeScale;
        PauseMenuUI.SetActive(false);
    }

    public void OnClickQuit()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = GlobalTimeScale;
        StartCoroutine(ExitScript());
    }

    public void OnCollectCoin()
    {
        coinCount++;
        if ((coinCount%Controller.CoinsNeededForExtraLive) == 0)
        {
            Controller.Lives++;
            OnLivesChange();
        }
        CointRenderer.SetText("x " + coinCount,true);
    }

    public void OnCollectLife()
    {
        Controller.Lives++;
        OnLivesChange();
    }

    public void OnCollectHealth()
    {
        if (Controller.Health < Controller.MaxHealth)
        {
            Controller.Health++;
            OnHealthChange();
        }
    }

    public void OnLivesChange()
    {
        LivesRenderer.SetText("x " + Controller.Lives, true);
    }

    public void OnHealthChange()
    {
        foreach (GameObject item in healthObjects)
        {
            Destroy(item);
        }
        healthObjects.Clear();
        for (int i = 0; i < Controller.MaxHealth; i++)
        {
            healthObjects.Add(Instantiate(HealthIcon, HealthContainer.transform,false));
            ((RectTransform)healthObjects[i].GetComponent(typeof(RectTransform))).anchoredPosition3D = new Vector3(50*i,0,0);
            if (Controller.Health > i) ((Image)healthObjects[i].GetComponent(typeof(Image))).color = HealthColor;
        }
    }

    IEnumerator ExitScript()
    {
        yield return ScreenFadeIn();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        yield return null;
    }

    IEnumerator EndLevel()
    {
        Controller.enabled = false;
        yield return ScreenFadeIn();
        EOLUI.SetActive(true);
        yield return ScreenFadeOut();
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
