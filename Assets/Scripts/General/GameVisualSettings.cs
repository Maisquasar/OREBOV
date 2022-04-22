using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameVisualSettings : MonoBehaviour
{

    [SerializeField] private Vector2[] _resolutionArray = new Vector2[0];
    private int _indexResolution = 0;
    public Vector2[] GameResolutions { get { return _resolutionArray; } }
    public int CurrentResolution { get { return _indexResolution; } }

    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = true;
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }

    private void SetScreenResolutions(Vector2 resolution)
    {
        Screen.SetResolution((int)resolution.x, (int)resolution.y, Screen.fullScreen);
    }

    public void IncreaseIndex()
    {
        _indexResolution++;
        _indexResolution = ManageIndex(_indexResolution, _resolutionArray.Length);
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }

    public void DecreaseIndex()
    {
        _indexResolution--;
        _indexResolution = ManageIndex(_indexResolution, _resolutionArray.Length);
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }


    private int ManageIndex(int currentIndex, int maxIndex)
    {
        int index = currentIndex;
        Debug.Log("Index =  " + index);
        if (currentIndex < 0) return index = maxIndex - 1;
        if (currentIndex >= maxIndex) return index = 0;
        return index;
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
