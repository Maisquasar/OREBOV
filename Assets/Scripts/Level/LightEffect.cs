using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffect : MonoBehaviour
{
    private Light _light;
    private int lastTime = 0;
    private float _intensity = 0;
    [SerializeField] private float _effectFrequency = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        _intensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        int tm = (int)(Time.time * 20);
        if (tm != lastTime)
        {
            if (Random.value < _effectFrequency)
            {
                _light.intensity = _intensity * Random.Range(0.7f, 1.3f);
            }
            lastTime = tm;
        }
    }
}
