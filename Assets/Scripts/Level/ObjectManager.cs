using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private InteractiveObject[] _interactiveObjectList = new InteractiveObject[0];


    public void FindAllObject()
    {
        _interactiveObjectList = FindObjectsOfType<InteractiveObject>();
    }

    public InteractiveObject ObjectsInRange(Vector3 position, Vector3 faceDir, float range, float angle)
    {
        InteractiveObject _item = null;
        float distanceObject = GameMetric.GetUnityValue(range);

        for (int i = 0; i < _interactiveObjectList.Length; i++)
        {
            Vector3 pos = _interactiveObjectList[i].transform.position + (position - _interactiveObjectList[i].transform.position).normalized * _interactiveObjectList[i].ObjectInteractionArea;
            if (Vector3.Distance(position, pos) < distanceObject)
            {
                Vector3 objDir = _interactiveObjectList[i].transform.position - position;
                if (Vector3.Dot(faceDir.normalized, objDir.normalized) > angle)
                {
                    distanceObject = Vector3.Distance(position, pos);
                    _item = _interactiveObjectList[i];
                }
            }
        }
        return _item;
    }

    public bool IsObjectInRange(Vector3 position, Vector3 faceDir, float range, float angle, InteractiveObject obj)
    {
        float distanceObject = GameMetric.GetUnityValue(range);
        if (Vector3.Distance(position, obj.transform.position) < distanceObject)
        {
            Vector3 objDir = obj.transform.position - position;
            if (Vector3.Dot(faceDir.normalized, objDir.normalized) > angle)
            {
                return true;
            }
        }
        return false;
    }
}
