using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : LootingEntity
{
    public Animator DragonAnimator;
    public GameObject DragonRenderer;
    public CapsuleCollider DragonMainCollider;
    public GameObject Player;

    public Material DefaultMaterial;
    public Material DamageMaterial;
    public SkinnedMeshRenderer DragonSkin;
    public ParticleSystem Particles;

    public float Speed = 1.0f;
    public float Range = 1.0f;
    public bool Attack = false;
    public float AngularVelocity = 120.0f;
    public float Bouncyness = 100.0f;
    public float DeltaPos = 0.0f;
    bool Dead = false;
    float OriginHeight;
    // Start is called before the first frame update
    void Start()
    {
        Particles.Stop();
        if (DragonAnimator == null) DragonAnimator = new Animator();
        if (DragonRenderer == null) DragonRenderer = new GameObject();
        if (DragonMainCollider == null) DragonMainCollider = new CapsuleCollider();
        OriginHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            if (Player.transform.position.x < transform.position.x)
            {
                if (DragonRenderer.transform.localRotation.eulerAngles.y < 270)
                {
                    DragonRenderer.transform.Rotate(new Vector3(0, 0, AngularVelocity * Time.deltaTime));
                }
            }
            else
            {
                if (DragonRenderer.transform.localRotation.eulerAngles.y > 90)
                {
                    DragonRenderer.transform.Rotate(new Vector3(0, 0, -AngularVelocity * Time.deltaTime));
                }
            }
            DeltaPos += Speed * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y = OriginHeight + Mathf.Sin(DeltaPos) * Range;
            transform.position = pos;
        }
        if (!Dead && transform.position.y < 0) KillEntity();
        DragonAnimator.SetBool("Attack", Attack);
        DragonAnimator.SetBool("Dead", Dead);
        Attack = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger && !Dead)
        {
            Component t = collider.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                PlayerController player = (PlayerController)t;
                if (player.DeathAnim) return;
                Vector3 playerVel = player.Body.velocity;
                playerVel.y = 0;
                player.Body.velocity = playerVel;
                player.Body.AddForce(transform.up * Bouncyness, ForceMode.Acceleration);
                KillEntity();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger && !Dead)
        {
            Component t = collision.collider.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                PlayerController player = (PlayerController)t;
                player.DamagePlayer();
                Attack = true;
            }
        }
    }

    public override void KillEntity()
    {
        Dead = true;
        DragonMainCollider.enabled = false;
        StartCoroutine(DragonKiller());
    }

    IEnumerator DragonKiller()
    {
        float time = 0.0f;
        if (DragonSkin && DamageMaterial) DragonSkin.material = DamageMaterial;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            yield return Time.deltaTime;
        }
        Particles.Play();
        if (DragonSkin && DefaultMaterial) DragonSkin.material = DefaultMaterial;
        while (time < 0.9f)
        {
            time += Time.deltaTime;
            yield return Time.deltaTime;
        }
        base.KillEntity();
        Destroy(transform.gameObject);
        yield return null;
    }

}
