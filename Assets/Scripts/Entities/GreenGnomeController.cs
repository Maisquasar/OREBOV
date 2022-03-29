using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GreenGnomeController : LootingEntity
{
    public Animator GnomeAnimator;
    public GameObject GnomeRenderer;
    public Rigidbody GnomeBody;
    public CapsuleCollider GnomeMainCollider;
    public GameObject Player;

    public Material DefaultMaterial;
    public Material DamageMaterial;
    public SkinnedMeshRenderer GnomeSkin;
    public ParticleSystem Particles;

    public float Speed = 1.0f;
    public bool Attack = false;
    public float AngularVelocity = 120.0f;
    public float Bouncyness = 100.0f;
    public float AttackRange = 5.0f;
    bool Dead = false;
    Vector3 oldPos;
    // Start is called before the first frame update
    void Start()
    {
        Particles.Stop();
        if (GnomeAnimator == null) GnomeAnimator = new Animator();
        if (GnomeRenderer == null) GnomeRenderer = new GameObject();
        if (GnomeBody == null) GnomeBody = new Rigidbody();
        if (GnomeMainCollider == null) GnomeMainCollider = new CapsuleCollider();
        oldPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead && Mathf.Abs(Player.transform.position.x - transform.position.x) < AttackRange)
        {
            if (Player.transform.position.x < transform.position.x)
            {
                if (GnomeRenderer.transform.localRotation.eulerAngles.y < 270)
                {
                    GnomeRenderer.transform.Rotate(new Vector3(0, AngularVelocity * Time.deltaTime, 0));
                }
                else
                {
                    transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                if (GnomeRenderer.transform.localRotation.eulerAngles.y > 90)
                {
                    GnomeRenderer.transform.Rotate(new Vector3(0, -AngularVelocity * Time.deltaTime, 0));
                }
                else
                {
                    transform.position += new Vector3(Speed * Time.deltaTime, 0, 0);
                }
            }
        }
        if (!Dead && transform.position.y < 0) KillEntity();
        GnomeAnimator.SetFloat("Speed", Mathf.Abs((transform.position - oldPos).x)/Time.deltaTime);
        GnomeAnimator.SetBool("Attack",Attack);
        GnomeAnimator.SetBool("Dead",Dead);
        Attack = false;
        oldPos = transform.position;
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
                player.Body.AddForce(transform.up * Bouncyness + GnomeBody.velocity, ForceMode.Acceleration);
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
        GnomeMainCollider.enabled = false;
        GnomeBody.useGravity = false;
        StartCoroutine(GnomeKiller());
    }

    IEnumerator GnomeKiller()
    {
        float time = 0.0f;
        if (GnomeSkin && DamageMaterial) GnomeSkin.material = DamageMaterial;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            yield return Time.deltaTime;
        }
        if (GnomeSkin && DefaultMaterial) GnomeSkin.material = DefaultMaterial;
        while (time < 1.6f)
        {
            time += Time.deltaTime;
            yield return Time.deltaTime;
        }
        Particles.Play();
        while (time < 2.5f)
        {
            time += Time.deltaTime;
            yield return Time.deltaTime;
        }
        base.KillEntity();
        Destroy(transform.gameObject);
        yield return null;
    }

}
