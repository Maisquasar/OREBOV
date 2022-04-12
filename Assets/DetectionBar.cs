using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBar : MonoBehaviour
{
    [SerializeField] Enemy _enemy;
    RectTransform _rectTransform;
    [SerializeField] Canvas _canvas;
    CameraBehavior _camera;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _camera = FindObjectOfType<CameraBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_rectTransform.sizeDelta.x <= 0)
            _canvas.enabled = false;
        else
            _canvas.enabled = true;

        _canvas.transform.rotation = _camera.transform.rotation;    


        Rect tmp = _rectTransform.rect;
        tmp.width = 1 - (_enemy.TimeStamp / _enemy.DetectionTime);
        _rectTransform.sizeDelta = new Vector2(tmp.width, tmp.height);
    }
}
