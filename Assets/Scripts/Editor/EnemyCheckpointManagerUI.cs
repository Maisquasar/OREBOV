//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyCheckpointManager))]
[CanEditMultipleObjects]
public class EnemyCheckpointManagerUI : Editor
{
    SerializedProperty TmpCheckpoints;

    void OnEnable()
    {
        TmpCheckpoints = serializedObject.FindProperty("Checkpoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Start Time
        GUILayout.BeginHorizontal();
        GUILayout.Label("First Checkpoint Time");
        Selection.activeGameObject.GetComponent<EnemyCheckpointManager>().StartTime = EditorGUILayout.FloatField(Selection.activeGameObject.GetComponent<EnemyCheckpointManager>().StartTime);
        GUILayout.EndHorizontal();

        // Enemy field
        GUILayout.BeginHorizontal();
        GUILayout.Label("Enemy");
        Selection.activeGameObject.GetComponent<EnemyCheckpointManager>()._enemy = EditorGUILayout.ObjectField(Selection.activeGameObject.GetComponent<EnemyCheckpointManager>()._enemy, typeof(MobileEnemy), true) as MobileEnemy;
        GUILayout.EndHorizontal();

        // Reverse field
        GUILayout.BeginHorizontal();
        Selection.activeGameObject.GetComponent<EnemyCheckpointManager>().Reverse = EditorGUILayout.Toggle("Reverse Movement", Selection.activeGameObject.GetComponent<EnemyCheckpointManager>().Reverse);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Detect Checkpoints"))
        {
            Selection.activeGameObject.GetComponent<EnemyCheckpointManager>().DetectCheckpoints();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        EditorGUILayout.PropertyField(TmpCheckpoints);
        serializedObject.ApplyModifiedProperties();
    }
}