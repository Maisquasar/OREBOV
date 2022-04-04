using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;

[ExecuteInEditMode]
public class LightManager : MonoBehaviour
{
    [SerializeField]
    public List<Light> TempLightsList;
    private static List<Light> LightsList;
    [SerializeField]
    public List<LightSubType> TempInteriorLightsList;
    private static List<LightSubType> InteriorLightsList;
    [SerializeField]
    public List<LightSubType> TempMovableLightsList;
    private static List<LightSubType> MovableLightsList;
    // Start is called before the first frame update
    void Start()
    {
        LightsList = new List<Light>();
        InteriorLightsList = new List<LightSubType>();
        MovableLightsList = new List<LightSubType>();
        if (TempLightsList == null) TempLightsList = new List<Light>();
        if (TempInteriorLightsList == null)  TempInteriorLightsList = new List<LightSubType>();
        if (TempMovableLightsList == null)  TempMovableLightsList = new List<LightSubType>();
        SynchronizeLists();
    }

    public void DetectLights()
    {
        TempLightsList.Clear();
        TempMovableLightsList.Clear();
        TempInteriorLightsList.Clear();
        foreach (Light item in FindObjectsOfType(typeof(Light)))
        {
            TempLightsList.Add(item);
            Component t = item.GetComponent(typeof(LightSubType));
            if (t != null)
            {
                LightSubType type = (LightSubType)t;
                if (type.Type == LightType.Interior || type.Type == LightType.InteriorMovable)
                {
                    TempInteriorLightsList.Add(type);
                }
                if (type.Type == LightType.Movable || type.Type == LightType.InteriorMovable)
                {
                    TempMovableLightsList.Add(type);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SynchronizeLists()
    {
        LightsList.Clear();
        LightsList.AddRange(TempLightsList);
        MovableLightsList.Clear();
        MovableLightsList.AddRange(TempMovableLightsList);
        InteriorLightsList.Clear();
        InteriorLightsList.AddRange(TempInteriorLightsList);
    }

    public static void SwitchToInterior()
    {
        foreach (LightSubType item in InteriorLightsList)
        {
            item.LightObject.color = item.SecondaryColor;
        }
    }

    public static void SwitchToExterior()
    {
        foreach (LightSubType item in InteriorLightsList)
        {
            item.LightObject.color = item.PrimaryColor;
        }
    }
}
