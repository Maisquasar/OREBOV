using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintScript : MonoBehaviour
{
    public float Bouncyness = 100.0f;
    public float BounceAnimHeight = 0.1f;
    public float PushAnimHeight = 0.05f;
    public GameObject SpringModel;
    public Rigidbody primaryBody;
    public float Cooldown = 0.2f;

    Coroutine bounceAnimation;
    Coroutine pushAnimation;
    float delay = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (SpringModel == null) SpringModel = new GameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > 0) delay -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delay <= 0.0f && !other.isTrigger)
        {
            Component t = other.GetComponent(typeof(Rigidbody));
            if (t != null)
            {
                Rigidbody body = ((Rigidbody)t);
                if (body.isKinematic || ((body.constraints & (RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX)) != 0))
                {
                    if (primaryBody == null) return;
                    primaryBody.AddForce(-transform.up * Bouncyness, ForceMode.Acceleration);
                    if (bounceAnimation != null)  StopCoroutine(bounceAnimation);
                    bounceAnimation = StartCoroutine(BounceAnim());
                }
                else
                {
                    Vector3 force = primaryBody == null ? (transform.up * Bouncyness) : (transform.up * Bouncyness + primaryBody.velocity);
                    body.AddForce(force, ForceMode.Acceleration);
                    if (bounceAnimation != null)  StopCoroutine(bounceAnimation);
                    bounceAnimation = StartCoroutine(BounceAnim());
                }
                delay = Cooldown;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.isTrigger && delay <= 0.0f)
        {
            Component t = other.collider.GetComponent(typeof(Rigidbody));
            if (t != null)
            {
                Rigidbody body = ((Rigidbody)t);
                if (!body.isKinematic && ((body.constraints & (RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX)) == 0))
                {
                    if (pushAnimation != null) StopCoroutine(pushAnimation);
                    pushAnimation = StartCoroutine(PushAnim());
                }
            }
        }
    }

    IEnumerator BounceAnim()
    {
        float delay = 2.0f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            Vector3 pos = SpringModel.transform.localPosition;
            pos.y = Mathf.Sin(10 * delay) * delay * BounceAnimHeight;
            SpringModel.transform.localPosition = pos;
            yield return Time.deltaTime;
        }
        bounceAnimation = null;
        yield return null;
    }

    IEnumerator PushAnim()
    {
        float delay = 2.0f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            Vector3 pos = SpringModel.transform.localPosition;
            pos.x = Mathf.Sin(10 * delay) * delay * PushAnimHeight;
            SpringModel.transform.localPosition = pos;
            yield return Time.deltaTime;
        }
        pushAnimation = null;
        yield return null;
    }
}
