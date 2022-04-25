using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIScriptCredits : MonoBehaviour
{
    // TODO :
    // - Fade out image
    [SerializeField] private float _fadeOutTime;
    [SerializeField] private Image _imageToFade;
    // - Move Text
    [SerializeField] private RectTransform _textToMove;
    [SerializeField] private float _speed;
    // - Check when to end
    [SerializeField] private float _creditTimer = 5f;
    // - Load NextScene
    [SerializeField] private int _sceneIndex;


    private void Start()
    {
        StartCoroutine(FadeOutImage());
    }

    private IEnumerator FadeOutImage()
    {
        float _timer = _fadeOutTime;
        Color _startColor = _imageToFade.color;
        float _startAlpha = _startColor.a;
        while (_timer > 0)
        {
            _startColor.a = Mathf.Lerp(_startAlpha, 0f, 1f - (_timer / _fadeOutTime));
            _imageToFade.color = _startColor;
            _timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }

        StartCoroutine(MoveText());
    }

    private IEnumerator MoveText()
    {
        float _timer = _creditTimer;
        while (_timer > 0)
        {
            _textToMove.transform.position += Vector3.up * Time.deltaTime * _speed;
            _timer -= Time.deltaTime; 
            yield return Time.deltaTime;
        }

        SceneManager.LoadScene(_sceneIndex);
    }

}
