using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeOrbScript : CollectableScript
{
    public ParticleSystem Particles;
    public GameObject SphereObject;
    public float RotationSpeedA = 70.0f;
    public float RotationSpeedB = 90.0f;
    public float RotationSpeedC = 30.0f;

    bool endAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f)));
    }

    // Update is called once per frame
    void Update()
    {
        if (!endAnimation)
        {
            transform.Rotate(new Vector3(RotationSpeedA * Time.deltaTime, RotationSpeedB * Time.deltaTime, RotationSpeedC * Time.deltaTime));
        }
    }

    public override void Collect(PauseMenuScript controller)
    {
        if (!endAnimation)
        {
            controller.OnCollectLife();
            StartCoroutine(DestroyAnimation());
        }
    }

    public IEnumerator DestroyAnimation()
    {
        endAnimation = true;
        SphereObject.SetActive(false);
        Particles.Play();
        float timer = 1.0f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        Destroy(gameObject);
        yield return null;
    }
}
