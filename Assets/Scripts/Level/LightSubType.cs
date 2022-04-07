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
    public bool IsBox = false;
    public Vector2 BoxSize = new Vector2();

    private void Update()
    {
        if (LightObject == null)
        {
            LightObject = (Light)gameObject.GetComponentInChildren(typeof(Light));
        }
        if (LightObject != null)
        {
            PrimaryColor = LightObject.color;
            if (IsBox && LightObject.areaSize.magnitude > 1 && LightObject.areaSize.magnitude != BoxSize.magnitude)
            {
                BoxSize = LightObject.areaSize;
                Debug.Log("S " + LightObject.areaSize);
            }
        }
    }
}
