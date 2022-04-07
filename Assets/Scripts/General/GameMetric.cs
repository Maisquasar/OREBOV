using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameMetric : MonoBehaviour
{
    private static float GameUnitInUnity = 2f;

    [SerializeField]
    private float _gameUnit;

    public void SetGameUnit()
    {
        GameUnitInUnity = _gameUnit;
    }

    private static float UnitRatio
    {
        get
        {
            return 1f / GameUnitInUnity;
        }
    }


    /// <summary>
    /// Transform game unit in Unity unit
    /// </summary>
    /// <param name="unit"> Set game unit </param>
    /// <returns></returns>
    public static float GetUnityValue(float unit)
    {
        return unit / UnitRatio;
    }

    /// <summary>
    /// Transform Unity unit in game unit
    /// </summary>
    /// <param name="unityValue"></param>
    /// <returns></returns>
    public static float GetGameUnit(float unityValue)
    {
        return unityValue * UnitRatio;
    }


}

