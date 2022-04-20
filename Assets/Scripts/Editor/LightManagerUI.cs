//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Threading;

[CustomEditor(typeof(LightManager))]
public class LightManagerUI : Editor
{
    SerializedProperty lightsList;
    SerializedProperty usableLightsList;
    SerializedProperty interiorLightsList;

    void OnEnable()
    {
        lightsList = serializedObject.FindProperty("TempLightsList");
        usableLightsList = serializedObject.FindProperty("TempUsableLightsList");
        interiorLightsList = serializedObject.FindProperty("TempInteriorLightsList");
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        if (GUILayout.Button("Detect Lights"))
        {
            Selection.activeGameObject.GetComponent<LightManager>().DetectLights();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        EditorGUILayout.PropertyField(lightsList);
        EditorGUILayout.PropertyField(usableLightsList);
        EditorGUILayout.PropertyField(interiorLightsList);
        /*
        // Special tool to help us later
        if (GUILayout.Button("You don't want to click me"))
        {
            Thread.Sleep(5000);
        }
        if (GUILayout.Button("You REALLY don't want to click me"))
        {
            EditorApplication.Exit(0);
        }
        */
        serializedObject.ApplyModifiedProperties();
    }
}