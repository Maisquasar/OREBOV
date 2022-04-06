using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightType
{
    Static = 0,
    Interior = 1,
    Usable = 2,
    Movable = 3,
    InteriorUsable = 3,
}

[ExecuteInEditMode]
public class LightSubType : MonoBehaviour
{
    public Light LightObject;
    public LightType Type = LightType.Static;
    public Color PrimaryColor;
    public Color SecondaryColor;
    public bool EjectPlayer = false;

    public void Start()
    {
        Component l = gameObject.GetComponentInChildren(typeof(Light));
        if (l != null)
        {
            LightObject = (Light)l;
        }
        Component t = gameObject.GetComponent(typeof(Light));
        if (t != null)
        {
            PrimaryColor = ((Light)t).color;
        }
    }
}
