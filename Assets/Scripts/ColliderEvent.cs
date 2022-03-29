using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    public UnityEvent<SurfaceType, bool> CollideEvent;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {

        bool collided = false;
        SurfaceType type = SurfaceType.None;
        float distance = 0.4f;
        for (int i = 0; i < 5; i++)
        {
            Vector3 position = transform.position;
            position.y += 0.3f;
            position.x = position.x - 0.3f + 0.15f * i;
            Debug.DrawRay(position, Vector3.down*distance, Color.yellow);
            Ray r = new Ray(position,Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, distance, 0xffffff, QueryTriggerInteraction.Ignore))
            {
                distance = hit.distance;
                collided = true;
                Component t = hit.collider.GetComponent(typeof(SubCollisionType));
                if (t != null)
                {
                    SubCollisionType subtype = (SubCollisionType)t;
                    type = subtype.type;
                }
                else
                {
                    type = SurfaceType.Default;
                }
            }
        }
        CollideEvent.Invoke(type,collided);
    }
}
