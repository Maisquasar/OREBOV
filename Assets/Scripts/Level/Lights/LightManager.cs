using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;

[ExecuteInEditMode]
public class LightManager : MonoBehaviour
{
    [SerializeField]
    List<Light> TempLightsList;
    private static List<Light> LightsList;
    [SerializeField]
    List<LightSubType> TempInteriorLightsList;
    private static List<LightSubType> InteriorLightsList;
    [SerializeField]
    List<LightSubType> TempUsableLightsList;
    private static List<LightSubType> UsableLightsList;
    // Start is called before the first frame update
    void Start()
    {
        if (TempLightsList == null) TempLightsList = new List<Light>();
        if (TempInteriorLightsList == null)  TempInteriorLightsList = new List<LightSubType>();
        if (TempUsableLightsList == null)  TempUsableLightsList = new List<LightSubType>();
        DetectLights();
    }

    public void DetectLights()
    {
        TempLightsList.Clear();
        TempUsableLightsList.Clear();
        TempInteriorLightsList.Clear();
        foreach (Light item in FindObjectsOfType<Light>(true))
        {
            TempLightsList.Add(item);
            Component t = item.GetComponent(typeof(LightSubType));
            if (t != null)
            {
                LightSubType type = (LightSubType)t;
                if (type.Type == LightType.Interior || type.Type == LightType.InteriorUsable)
                {
                    TempInteriorLightsList.Add(type);
                }
                if (type.Type == LightType.Usable || type.Type == LightType.InteriorUsable)
                {
                    TempUsableLightsList.Add(type);
                }
            }
        }
        SynchronizeLists();
    }

    // Update is called once per frame
    void Update()
    {
        if (LightsList == null)
        {
            Start();
        }
    }

    private void SynchronizeLists()
    {
        LightsList = new List<Light>();
        InteriorLightsList = new List<LightSubType>();
        UsableLightsList = new List<LightSubType>();
        LightsList.AddRange(TempLightsList);
        UsableLightsList.AddRange(TempUsableLightsList);
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

    public static LightSubType[] GetUsableLights()
    {
        if (UsableLightsList == null) return new LightSubType[0];
        return UsableLightsList.ToArray();
    }
}
