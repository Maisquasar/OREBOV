using System.Collections.Generic;
using UnityEngine;


public class ShadowCaster : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool ShowAllRays = false;
    [SerializeField] List<Vector2> ShadowRayCastPosition;
    [SerializeField] List<Vector2> DepthRayCastPosition;
    [SerializeField] float HitBoxRadius = 0.5f;
    [SerializeField] float ShadowHeightDeltaMin = 0.2f;
    private bool[] _pointsHit;
    private Vector3[] _pointsPos;
    private float _shadowDepth;
    private float _shadowHeight;
    public float ShadowDepth { get { return _shadowDepth - HitBoxRadius - 1; } }
    public float ShadowHeight { get { return _shadowHeight; } }
    public bool DoesCurrentLightEject = false;
    private int _mask = 0;
    private int _maskDepth = 0;

    private bool getLightVectors(LightSubType obj, Vector2 delta, out Vector3 origin, out Vector3 direction)
    {
        origin = new Vector3();
        direction = new Vector3();
        Light item = obj.LightObject;
        if (item.isActiveAndEnabled && item.type == UnityEngine.LightType.Spot)
        {
            Vector3 EVPos = transform.position + new Vector3(delta.x, delta.y, 0);
            if (obj.IsBox)
            {
                float lPosX = -Vector3.Dot(item.transform.position - EVPos, item.transform.right);
                float lPosY = -Vector3.Dot(item.transform.position - EVPos, item.transform.up);
                if (Mathf.Abs(lPosX) < obj.BoxSize.x / 2 && Mathf.Abs(lPosY) < obj.BoxSize.y / 2)
                {
                    origin = item.transform.position + item.transform.right * lPosX + item.transform.up * lPosY;
                    direction = EVPos - origin;
                    return true;
                }
            }
            else
            {
                if (item.spotAngle / 2 > Vector3.Angle(item.transform.forward, EVPos - item.transform.position))
                {
                    origin = item.transform.position;
                    direction = EVPos - item.transform.position;
                    return true;
                }
            }
        }
        return false;
    }
    protected void OnDrawGizmos()
    {
        LightSubType[] lights = LightManager.GetUsableLights();
        for (int i = 0; i < lights.Length; i++)
        {
            //Light item = lights[i].LightObject;
            Vector3 origin;
            Vector3 direction;
            if (getLightVectors(lights[i], new Vector2(), out origin, out direction))
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawRay(origin, direction);
                    if (ShowAllRays)
                    {
                        CanTransform(true);
                        Gizmos.color = Color.yellow;
                        for (int j = 0; j < ShadowRayCastPosition.Count; j++)
                        {
                            if (_pointsHit[j]) continue;
                            Gizmos.DrawRay(_pointsPos[j], transform.position - _pointsPos[j] + new Vector3(ShadowRayCastPosition[j].x, ShadowRayCastPosition[j].y, 0));
                        }
                    }
                    break;
                }
        }
        Gizmos.color = Color.yellow;
        for (int j = 0; j < ShadowRayCastPosition.Count; j++)
        {
            Gizmos.DrawSphere(transform.position + new Vector3(ShadowRayCastPosition[j].x, ShadowRayCastPosition[j].y, 0), 0.05f);
        }
        Gizmos.color = Color.green;
        for (int j = 0; j < DepthRayCastPosition.Count; j++)
        {
            Gizmos.DrawSphere(transform.position + new Vector3(DepthRayCastPosition[j].x, DepthRayCastPosition[j].y, 0), 0.05f);
        }
    }

    /// <summary>
    /// Test if player can transform to shadow:
    /// We first list all usable lights, then we see if we are in range for one (not outside of it's angle) by testing with
    /// If we are on a spotLight
    /// </summary>
    /// <returns></returns>
    public bool CanTransform(bool ShouldBeInFront)
    {
        _shadowDepth = 100000;
        if (_mask == 0) _mask = 0x7fffffff - LayerMask.GetMask("TransparentFX", "Ignore Raycast", "NoShadows");
        if (_maskDepth == 0) _maskDepth = 0x7fffffff - LayerMask.GetMask("TransparentFX", "Ignore Raycast", "Shadows", "NoShadows");
        if (_pointsHit == null || _pointsHit.Length != ShadowRayCastPosition.Count) _pointsHit = new bool[ShadowRayCastPosition.Count];
        if (_pointsPos == null || _pointsPos.Length != ShadowRayCastPosition.Count) _pointsPos = new Vector3[ShadowRayCastPosition.Count];
        for (int i = 0; i < _pointsHit.Length; i++) _pointsHit[i] = true;
        LightSubType[] lights = LightManager.GetUsableLights();
        for (int i = 0; i < lights.Length; i++)
        {
            Vector3 origin;
            Vector3 direction;
            if (getLightVectors(lights[i], new Vector2(), out origin, out direction))
            {
                float dist = direction.magnitude;
                bool hit = false;
                for (int j = 0; j < ShadowRayCastPosition.Count; j++)
                {
                    if (_pointsHit[j] == false) continue;
                    RaycastHit rayHit;
                    if (!getLightVectors(lights[i], ShadowRayCastPosition[j], out origin, out direction) || !Physics.Raycast(origin, direction, out rayHit, 10000, _mask, QueryTriggerInteraction.Ignore) || rayHit.distance < (ShouldBeInFront ? dist : 0))
                    {
                        hit = true;
                    }
                    else
                    {
                        DoesCurrentLightEject = lights[i].EjectPlayer;
                        _pointsHit[j] = false;
                        _pointsPos[j] = origin;
                    }
                }
                for (int j = 0; j < DepthRayCastPosition.Count; j++)
                {
                    RaycastHit rayHit;
                    if (getLightVectors(lights[i], DepthRayCastPosition[j], out origin, out direction) && Physics.Raycast(origin, direction, out rayHit, 10000, _maskDepth, QueryTriggerInteraction.Ignore) && rayHit.distance > 0)
                    {
                        if (rayHit.point.z < _shadowDepth) _shadowDepth = rayHit.point.z;
                        if (Mathf.Abs(rayHit.point.y - _shadowHeight) > ShadowHeightDeltaMin) _shadowHeight = rayHit.point.y - DepthRayCastPosition[j].y;
                    }
                }
                if (!hit) return true;
            }
        }
        for (int i = 0; i < _pointsHit.Length; i++)
        {
            if (_pointsHit[i]) return false;
        }
        return true;
    }

    public Vector3 GetShadowPos()
    {
        LightSubType[] lights = LightManager.GetUsableLights();
        for (int i = 0; i < lights.Length; i++)
        {
            Vector3 origin;
            Vector3 direction;
            if (getLightVectors(lights[i],new Vector2(), out origin, out direction))
            {
                RaycastHit rayHit;
                if (Physics.Raycast(item.transform.position, transform.position - item.transform.position, out rayHit, 10000, _mask, QueryTriggerInteraction.Ignore) && rayHit.distance > 0)
                {
                    Vector3 outValue = rayHit.point + Vector3.back * HitBoxRadius;
                    if (Mathf.Abs(outValue.y-transform.position.y) < ShadowHeightDeltaMin) outValue.y = transform.position.y;
                    return outValue;
                }
            }
        }
        return new Vector3();
    }
}
