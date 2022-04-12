using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PointsInterestManager : MonoBehaviour
{
    [SerializeField] private PointsInterest[] _pointsInterestsArray = new PointsInterest[0];
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private CameraBehavior _camereBehavior;



    private float GetWidth()
    {
        Vector3 target = _camereBehavior.MainTarget.transform.position;
        Vector3 cam = _camereBehavior.transform.position;

        float distance = target.z - cam.z; 
        float height =  distance * Mathf.Tan(_mainCamera.fieldOfView  * 0.5f * Mathf.Deg2Rad);
        return height * _mainCamera.aspect;
    }
        
    // Check if and object is front the player on the axis X
    private bool IsObjectFrontPlayer(Vector3 positionObj)
    {
        if (_camereBehavior.MainTarget.transform.position.x > positionObj.x) return true;
        else return false;
    }   

    private void IsInRange(Vector3 positionObj)
    {
        if (IsObjectFrontPlayer(positionObj))
        {
            
        }
    }


    public void OnDrawGizmos()
    {
        Vector3 pos = new Vector3(_mainCamera.transform.position.x + GetWidth(), _camereBehavior.MainTarget.transform.position.y, _camereBehavior.MainTarget.transform.position.z)  - _mainCamera.transform.position;
        Debug.Log(pos.normalized);
        Gizmos.DrawLine(_mainCamera.transform.position , _mainCamera.transform.position+ _mainCamera.transform.rotation* pos.normalized * GetWidth());
        Gizmos.DrawSphere(_mainCamera.transform.position + _mainCamera.transform.rotation * pos.normalized * GetWidth(), 0.1f);
    }
}
