using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GnomeStates
{
    IdleLeft = 0,
    MoveRight,
    IdleRight,
    MoveLeft,
    Dead,
}

public class RedGnomeController : LootingEntity
{
    public Animator GnomeAnimator;
    public GameObject GnomeRenderer;
    public Rigidbody GnomeBody;
    public CapsuleCollider GnomeMainCollider;

    public Material DefaultMaterial;
    public Material DamageMaterial;
    public SkinnedMeshRenderer GnomeSkin;
    public ParticleSystem Particles;

    public float Speed = 1.0f;
    public bool Attack = false;
    public GnomeStates GnomeStatus = GnomeStates.IdleLeft;
    public float AngularVelocity = 120.0f;
    public float Bouncyness = 100.0f;
    public float WaitDelay = 1.0f;
    public float Timer = 0.0f;
    bool stop = false;
    Vector3 oldPos = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        Particles.Stop();
        if (GnomeAnimator == null) GnomeAnimator = new Animator();
        if (GnomeRenderer == null) GnomeRenderer = new GameObject();
        if (GnomeBody == null) GnomeBody = new Rigidbody();
        if (GnomeMainCollider == null) GnomeMainCollider = new CapsuleCollider();
    }

    // Update is called once per frame
    void Update()
    {
        float doesMove = 0.0f;
        switch (GnomeStatus)
        {
            case GnomeStates.IdleLeft:
                if (IdleWait())
                {
                    Attack = false;
                    GnomeStatus = GnomeStates.MoveRight;
                }
                break;
            case GnomeStates.MoveRight:
                if (GnomeRenderer.transform.localRotation.eulerAngles.y > 90)
                {
                    GnomeRenderer.transform.Rotate(new Vector3(0,-AngularVelocity*Time.deltaTime,0));
                    stop = false;
                }
                else
                {
                    Vector3 tmp = transform.position;
                    transform.position += new Vector3(Speed*Time.deltaTime,0,0);
                    doesMove = 1.0f;
                    if (detectVoid() || stop || (tmp - transform.position).magnitude < Speed * Time.deltaTime * 0.9f || (oldPos - transform.position).magnitude < Speed * Time.deltaTime * 0.9f)
                    {
                        GnomeStatus = GnomeStates.IdleRight;
                        stop = false;
                        oldPos = new Vector3();
                    }
                    else
                        oldPos = tmp;
                }
                break;
            case GnomeStates.IdleRight:
                if (IdleWait())
                {
                    GnomeStatus = GnomeStates.MoveLeft;
                    Attack = false;
                }
                break;
            case GnomeStates.MoveLeft:
                if (GnomeRenderer.transform.localRotation.eulerAngles.y < 270)
                {
                    GnomeRenderer.transform.Rotate(new Vector3(0, AngularVelocity * Time.deltaTime, 0));
                    stop = false;
                }
                else
                {
                    Vector3 tmp = transform.position;
                    transform.position += new Vector3(-Speed * Time.deltaTime, 0, 0);
                    doesMove = 1.0f;
                    if (detectVoid() || stop || (tmp-transform.position).magnitude < Speed * Time.deltaTime * 0.9f || (tmp - transform.position).magnitude < Speed * Time.deltaTime * 0.9f)
                    {
                        GnomeStatus = GnomeStates.IdleLeft;
                        stop = false;
                        oldPos = new Vector3();
                    }
                    else
                        oldPos = tmp;
                }
                break;
            default:
                GnomeBody.position += GnomeBody.velocity * Time.deltaTime;
                break;
        }
        if (GnomeStatus != GnomeStates.Dead && transform.position.y < 0) KillEntity();
        GnomeAnimator.SetFloat("Speed", doesMove);
        GnomeAnimator.SetBool("Attack",Attack);
        GnomeAnimator.SetBool("Dead",GnomeStatus == GnomeStates.Dead);
    }

    bool IdleWait()
    {
        Timer += Time.deltaTime;
        if (Timer > WaitDelay)
        {
            Timer = 0.0f;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger && GnomeStatus != GnomeStates.Dead)
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
        if (!collision.collider.isTrigger && GnomeStatus != GnomeStates.Dead)
        {
            Component t = collision.collider.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                PlayerController player = (PlayerController)t;
                player.DamagePlayer();
                Timer = 0;
                if (collision.collider.transform.position.x > transform.position.x)
                {
                    GnomeStatus = GnomeStates.IdleRight;
                }
                else
                {
                    GnomeStatus = GnomeStates.IdleLeft;
                }
                Attack = true;

            }
            else if (GnomeStatus == GnomeStates.MoveLeft || GnomeStatus == GnomeStates.MoveRight)
            {
                stop = true;
            }
        }
    }

    bool detectVoid()
    {
        Ray r1 = new Ray(transform.position + new Vector3(-0.3f, 0.2f, 0), Vector3.down);
        Ray r2 = new Ray(transform.position + new Vector3(0.3f, 0.2f, 0), Vector3.down);
        Debug.DrawRay(r1.origin, Vector3.down * 0.5f, Color.yellow);
        Debug.DrawRay(r2.origin, Vector3.down * 0.5f, Color.yellow);
        bool left = Physics.Raycast(r1, 0.5f, 0xffffff, QueryTriggerInteraction.Ignore) || GnomeStatus == GnomeStates.MoveRight;
        bool right = Physics.Raycast(r2, 0.5f, 0xffffff, QueryTriggerInteraction.Ignore) || GnomeStatus == GnomeStates.MoveLeft;
        
        return !(left && right);
    }

    public override void KillEntity()
    {
        GnomeStatus = GnomeStates.Dead;
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
