using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrollStates
{
    Idle = 0,
    BigAttack,
    BigAttackWait,
    BallsMode,
    Dead,
}

public class TrollScript : LootingEntity
{
    public TrollHeadScript HeadPart;
    public TrollClubScript ClubPart;
    public TrollBodyScript BodyPart;
    public Animator TrollAnimator;
    public Material DefaultMaterial;
    public Material DamageMaterial;
    public SkinnedMeshRenderer TrollSkin;
    public ParticleSystem Particles;
    public GameObject BaseSpawn;
    public GameObject RareSpawn;
    public GameObject BallSpawner;
    public float RareSpawnChance = 0.2f;
    public Vector3 SpawnVelocity = new Vector3(-2,0.5f,0);

    public float IdleTimer = 3.0f;
    public float BallsTimer = 10.0f;
    public float WaitTimer = 5.0f;
    public float BigAttackChances = 0.5f;
    public float BallsSpawnInterval = 1.0f;
    public int MaxLife = 3;

    TrollStates TrollState;
    Collider[] colliders;
    float timer;
    int life;
    bool hasBeenBallsMode = false;

    // Start is called before the first frame update
    void Start()
    {
        colliders = new Collider[5];
        colliders[0] = HeadPart.getHitBox();
        colliders[1] = HeadPart.getTrigger();
        colliders[2] = ClubPart.getHitBox();
        colliders[3] = BodyPart.getHitBox();
        colliders[4] = BodyPart.getTrigger();

        for (int i = 0; i < 4; i++)
            for (int j = i + 1; j < 5; j++)
                Physics.IgnoreCollision(colliders[i], colliders[j], true);
        timer = IdleTimer;
        life = MaxLife;
        TrollState = TrollStates.Idle;
        ClubPart.Active = true;
        BodyPart.Active = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (TrollState)
        {
            case TrollStates.Idle:
                {
                    if (timer > 0.0f)
                    {
                        timer -= Time.deltaTime;
                    }
                    else
                    {
                        if (hasBeenBallsMode || Random.value > BigAttackChances)
                        {
                            hasBeenBallsMode = false;
                            timer = 1.5f; // Hit Animation Length
                            // Todo Damage Player on ground
                            TrollState = TrollStates.BigAttack;
                            TrollAnimator.SetBool("BigAttack", true);
                        }
                        else
                        {
                            hasBeenBallsMode = true;
                            timer = BallsTimer;
                            TrollState = TrollStates.BallsMode;
                            TrollAnimator.SetBool("BallsMode", true);
                            StartCoroutine(BallSpawnAnimation(BallsTimer, BallsSpawnInterval));
                        }
                    }
                    break;
                }
            case TrollStates.BigAttack:
                {
                    if (timer > 0.0f)
                    {
                        timer -= Time.deltaTime;
                    }
                    else
                    {
                        timer = WaitTimer;
                        TrollState = TrollStates.BigAttackWait;
                        HeadPart.Touched = false;
                        BodyPart.Touched = false;
                        BodyPart.Active = false;
                        ClubPart.Active = false;
                    }
                    break;
                }
            case TrollStates.BigAttackWait:
                {
                    if (timer > 0.0f)
                    {
                        timer -= Time.deltaTime;
                        if (HeadPart.Touched || BodyPart.Touched)
                        {
                            life--;
                            StartCoroutine(DamageAnimation());
                            if (life <= 0)
                            {
                                TrollState = TrollStates.Dead;
                                TrollAnimator.SetBool("Dead", true);
                                StartCoroutine(TrollKiller());
                            }
                            else
                            {
                                timer = IdleTimer;
                                TrollState = TrollStates.Idle;
                                HeadPart.Touched = false;
                                BodyPart.Touched = false;
                                BodyPart.Active = true;
                                ClubPart.Active = true;
                                TrollAnimator.SetBool("BigAttack", false);
                            }
                        }
                    }
                    else
                    {
                        timer = IdleTimer;
                        TrollState = TrollStates.Idle;
                        HeadPart.Touched = false;
                        BodyPart.Touched = false;
                        BodyPart.Active = true;
                        ClubPart.Active = true;
                        TrollAnimator.SetBool("BigAttack", false);
                    }
                    break;
                }
            case TrollStates.BallsMode:
                {
                    if (timer > 0.0f)
                    {
                        timer -= Time.deltaTime;
                    }
                    else
                    {
                        timer = IdleTimer;
                        TrollState = TrollStates.Idle;
                        TrollAnimator.SetBool("BallsMode", false);
                    }
                    break;
                }
            default:
                {
                    BodyPart.Active = false;
                    ClubPart.Active = false;
                }
                break;
        }
    }

    public override void KillEntity()
    {
        if (TrollState != TrollStates.Dead)
        {
            life = 0;
            TrollState = TrollStates.Dead;
            TrollAnimator.SetBool("Dead", true);
            StartCoroutine(TrollKiller());
        }
    }

    IEnumerator DamageAnimation()
    {
        float timer = 0.2f;
        TrollSkin.material = DamageMaterial;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
        TrollSkin.material = DefaultMaterial;
        yield return null;
    }

    IEnumerator BallSpawnAnimation(float timer, float deltaTime)
    {
        timer /= deltaTime;
        int count = (int)timer;
        while (timer > 0)
        {
            timer -= Time.deltaTime * deltaTime;
            int tmp = (int)timer;
            if (tmp != count)
            {
                GameObject spawn;
                count = tmp;
                if (Random.value < RareSpawnChance)
                {
                    spawn = Instantiate(RareSpawn, BallSpawner.transform, false);
                }
                else
                {
                    spawn = Instantiate(BaseSpawn, BallSpawner.transform, false);
                }
                ((Rigidbody)spawn.GetComponent(typeof(Rigidbody))).velocity = SpawnVelocity;
            }
            yield return Time.deltaTime;
        }
        yield return null;
    }

    IEnumerator TrollKiller()
    {
        float timer = 0.0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        Particles.Play();
        while (timer < 3.0f)
        {
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        base.KillEntity();
        Destroy(transform.gameObject);
        yield return null;
    }
}
