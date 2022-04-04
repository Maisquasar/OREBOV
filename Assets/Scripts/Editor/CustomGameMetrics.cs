using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

    [CustomEditor(typeof(GameMetric))]
public class CustomGameMetrics : Editor
{
    SerializedProperty _unit;

    private void OnEnable()
    {
        _unit = serializedObject.FindProperty("_gameUnit");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_unit);
        Selection.activeGameObject.GetComponent<GameMetric>().SetGameUnit();
        serializedObject.ApplyModifiedProperties();
    }
}
