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
            if (Vector3.Distance(position, _interactiveObjectList[i].transform.position) < distanceObject)
            {
                Vector3 objDir = _interactiveObjectList[i].transform.position - position;
                if (Vector3.Dot(faceDir.normalized, objDir.normalized) > angle)
                { 
                    distanceObject = Vector3.Distance(position, _interactiveObjectList[i].transform.position);
                    _item = _interactiveObjectList[i];

                }
            }
        }
        return _item;
    }

}
