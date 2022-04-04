using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralMenuFeature : MonoBehaviour
{
    public void RestartScene()
    {
        Debug.Log("Reload the Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit the scene");
        Application.Quit();
    }
}
