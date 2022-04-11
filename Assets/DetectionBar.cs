using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBar : MonoBehaviour
{
    [SerializeField] Enemy _enemy;
    RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Rect tmp = _rectTransform.rect;
        tmp.width = 1 - (_enemy.TimeStamp / _enemy.DetectionTime);
        _rectTransform.sizeDelta = new Vector2(tmp.width, tmp.height);
    }
}
