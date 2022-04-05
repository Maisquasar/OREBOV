using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectManager))]
public class CustomObjectManager : Editor
{
    SerializedProperty _listItem;
    
    private void OnEnable()
    {
        _listItem = serializedObject.FindProperty("_interactiveObjectList");
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        EditorGUILayout.PropertyField(_listItem);
        if(GUILayout.Button("Find Objects"))
        {
            Selection.activeGameObject.GetComponent<ObjectManager>().FindAllObject();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        serializedObject.ApplyModifiedProperties();
    }

 
}
