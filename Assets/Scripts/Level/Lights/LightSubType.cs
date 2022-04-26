using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

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

    public HDAdditionalLightData data;

    [SerializeField]
    public bool IsBox = false;
    public Vector2 BoxSize = new Vector2();

    private void Start()
    {
        BoxSize = new Vector2(data.shapeWidth, data.shapeHeight);
    }

    private void Update()
    {
        if (LightObject == null)
        {
            LightObject = (Light)gameObject.GetComponentInChildren(typeof(Light));
        }
        if (LightObject != null)
        {
            PrimaryColor = LightObject.color;
            Vector2 area = new Vector2(data.shapeWidth, data.shapeHeight);
            if (IsBox && area.magnitude != BoxSize.magnitude)
            {
                BoxSize = new Vector2(data.shapeWidth, data.shapeHeight);
            }
        }
    }
}
