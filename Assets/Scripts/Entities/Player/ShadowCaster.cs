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
    private float _shadowDepth;
    private float _shadowHeight;
    public float ShadowDepth { get { return _shadowDepth - HitBoxRadius - 1; } }
    public float ShadowHeight { get { return _shadowHeight; } }

    public bool DoesCurrentLightEject = false;
    private int _mask = 0;
    private int _maskDepth = 0;

    protected void OnDrawGizmos()
    {
        LightSubType[] lights = LightManager.GetUsableLights();
        for (int i = 0; i < lights.Length; i++)
        {
            Light item = lights[i].LightObject;
            if (item.type == UnityEngine.LightType.Spot && item.spotAngle / 2 > Vector3.Angle(item.transform.forward, transform.position - item.transform.position))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(item.transform.position, transform.position - item.transform.position);
                if (ShowAllRays)
                {
                    CanTransform(true);
                    for (int j = 0; j < ShadowRayCastPosition.Count; j++)
                    {
                        if (_pointsHit[j]) Gizmos.color = Color.red;
                        else Gizmos.color = Color.yellow;
                        Gizmos.DrawRay(item.transform.position, transform.position + new Vector3(ShadowRayCastPosition[j].x, ShadowRayCastPosition[j].y, 0) - item.transform.position);
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
        for (int i = 0; i < _pointsHit.Length; i++) _pointsHit[i] = true;
        LightSubType[] lights = LightManager.GetUsableLights();
        for (int i = 0; i < lights.Length; i++)
        {
            Light item = lights[i].LightObject;
            if (item.type == UnityEngine.LightType.Spot && item.spotAngle / 2 > Vector3.Angle(item.transform.forward, transform.position - item.transform.position))
            {
                float dist = (transform.position - item.transform.position).magnitude;
                bool hit = false;
                for (int j = 0; j < ShadowRayCastPosition.Count; j++)
                {
                    if (_pointsHit[j] == false) continue;
                    Vector3 deltaPos = transform.position + new Vector3(ShadowRayCastPosition[j].x, ShadowRayCastPosition[j].y, 0);
                    Vector3 dir = (deltaPos - item.transform.position).normalized;
                    RaycastHit rayHit;
                    if (item.spotAngle / 2 <= Vector3.Angle(item.transform.forward, dir) || !Physics.Raycast(item.transform.position, dir, out rayHit, 10000, _mask, QueryTriggerInteraction.Ignore) || rayHit.distance < (ShouldBeInFront ? dist : 0))
                    {
                        hit = true;
                    }
                    else
                    {
                        DoesCurrentLightEject = lights[i].EjectPlayer;
                        _pointsHit[j] = false;
                    }
                }
                for (int j = 0; j < DepthRayCastPosition.Count; j++)
                {
                    Vector3 deltaPos = transform.position + new Vector3(DepthRayCastPosition[j].x, DepthRayCastPosition[j].y, 0);
                    Vector3 dir = (deltaPos - item.transform.position).normalized;
                    RaycastHit rayHit;
                    if (item.spotAngle / 2 >= Vector3.Angle(item.transform.forward, dir) && Physics.Raycast(item.transform.position, dir, out rayHit, 10000, _maskDepth, QueryTriggerInteraction.Ignore) && rayHit.distance > 0)
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
            Light item = lights[i].LightObject;
            if (item.type == UnityEngine.LightType.Spot && item.spotAngle / 2 > Vector3.Angle(item.transform.forward, transform.position - item.transform.position))
            {
                float dist = (transform.position - item.transform.position).magnitude;
                RaycastHit rayHit;
                if (Physics.Raycast(item.transform.position, transform.position - item.transform.position, out rayHit, 10000, _maskDepth, QueryTriggerInteraction.Ignore) && rayHit.distance > 0)
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
