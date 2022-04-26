using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct ScreenSize
{
    public readonly int width;
    public readonly int height;

    public ScreenSize(int w, int h) : this()
    {
        width = w;
        height = h;
    }
}


public class GameVisualSettings : MonoBehaviour
{

    [SerializeField] private List<ScreenSize> _resolutionArray = new List<ScreenSize>();
    private int _indexResolution = 0;
    public ScreenSize[] GameResolutions { get { return _resolutionArray.ToArray(); } }
    public int CurrentResolution { get { return _indexResolution; } }

    // Start is called before the first frame update
    void Start()
    {
        Resolution[] _res = Screen.resolutions;
        for (int i = 0; i < _res.Length; i++)
        {
            _resolutionArray.Add(new ScreenSize(_res[i].width, _res[i].height));
        }
        _resolutionArray = new List<ScreenSize>(_resolutionArray.Distinct().ToArray());
        _indexResolution = _resolutionArray.Count - 1;
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }

    private void SetScreenResolutions(ScreenSize resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void IncreaseIndex()
    {
        Debug.Log("Test");
        _indexResolution++;
        _indexResolution = ManageIndex(_indexResolution, _resolutionArray.Count);
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }

    public void DecreaseIndex()
    {
        _indexResolution--;
        _indexResolution = ManageIndex(_indexResolution, _resolutionArray.Count);
        SetScreenResolutions(_resolutionArray[_indexResolution]);
    }


    private int ManageIndex(int currentIndex, int maxIndex)
    {
        int index = currentIndex;
        if (currentIndex < 0) return  maxIndex - 1;
        if (currentIndex >= maxIndex) return 0;
        return index;
    }
    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
