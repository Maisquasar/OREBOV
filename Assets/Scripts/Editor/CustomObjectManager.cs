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
        ObjectManager objectManager = (ObjectManager)target;
        EditorUtility.ClearDirty(serializedObject.targetObject);
        serializedObject.Update();
        EditorGUILayout.PropertyField(_listItem);
        if(GUILayout.Button("Find Objects"))
        {
            objectManager.FindAllObject();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        serializedObject.ApplyModifiedProperties();
    }

 
}
