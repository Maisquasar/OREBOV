using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsInterest : MonoBehaviour
{
    [Range(0,10f)]
    [SerializeField] private float _importance;
        
    public float GetWeight(float distance)
    {
        return _importance * distance;
    }
}
