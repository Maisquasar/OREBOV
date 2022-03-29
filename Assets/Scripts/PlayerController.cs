using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    public float Acceleration = 150.0f;
    public float MaxSpeed = 10;
    public float AirAcceleration = 50.0f;
    public float JumpPower = 1.5f;
    public float AngularVelocity = 1000.0f;
    public float PlayerFriction = 0.05f;
    public float PlayerIceFriction = 0.01f;
    public float PlayerStopSpeed = 1.0f;
    public int Lives = 2;
    public int Health = 5;
    public int MaxHealth = 5;
    public int CoinsNeededForExtraLive = 50;
    public Rigidbody Body;
    public Animator Anim;
    public Transform Model;
    public Ragdollhandler Ragdoll;
    public Material DefaultMaterial;
    public Material DamageMaterial;
    public SkinnedMeshRenderer PlayerRendererSkin;
    public Vector3 RespawnPoint;
    public PauseMenuScript UIHandler;
    public GameObject MainCamera;
    public GameObject FPSCamera;
    public Vector2 CameraDeltaMovement = new Vector2(1,3);
    public float CameraDeltaRotation = 40;
    public float CameraDeltaSpeed = 3.0f;
    public float CameraDeltaAngularSpeed = 3.0f;
    public float DeathFloor = -1.0f;

    public float StickDeadZone = 0.03f;

    Vector2 movementDir = new Vector2();
    Vector2 dir = new Vector2();
    float targetRotation = 0.0f;
    Vector3 camPosition = new Vector3(10,1,0);
    Vector3 targetCamPosition = new Vector3(0,0,2);
    float targetCamRotation = 0.0f;
    bool onGroung = true;
    bool deathAnim = false;
    bool stunt = false;
    bool damageable = true;
    public SurfaceType walkOn = SurfaceType.None;
    public bool DeathAnim { get { return deathAnim; } }
    // Start is called before the first frame update
    void Start()
    {
        if (Body == null) Body = GetComponent<Rigidbody>();
        if (Model == null) Model = transform.GetChild(1);
        if (Anim == null) Anim = Model.GetComponent<Animator>();
        Body.freezeRotation = true;
        if (MainCamera == null) MainCamera = new GameObject();
        if (FPSCamera == null) FPSCamera = new GameObject();
        camPosition = MainCamera.transform.localPosition;
        RespawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stunt && !deathAnim)
        {
            dir = movementDir;
        }
        else
        {
            dir.Set(0, 0);
        }
        targetCamPosition.z = targetRotation == 0.0f ? CameraDeltaMovement.x : -CameraDeltaMovement.x;
        targetCamPosition.y = dir.y * CameraDeltaMovement.y;
        targetCamRotation = -dir.y * CameraDeltaRotation;
        if (dir.x > 0)
        {
            if (onGroung)
            {
                if (Body.velocity.x > 0.05f) targetRotation = 0.0f;
                Body.AddForce(new Vector3(Acceleration * Time.deltaTime, 0, 0), ForceMode.Force);
            }
            else
            {
                Body.AddForce(new Vector3(AirAcceleration * Time.deltaTime, 0, 0), ForceMode.Force);
            }
        }
        else if (dir.x < 0)
        {
            if (onGroung)
            {
                if (Body.velocity.x < -0.05f) targetRotation = 180.0f;
                Body.AddForce(new Vector3(-Acceleration * Time.deltaTime, 0, 0), ForceMode.Force);
            }
            else
            {
                Body.AddForce(new Vector3(-AirAcceleration * Time.deltaTime, 0, 0), ForceMode.Force);
            }
        }
        if (onGroung)
        {
            Body.velocity = new Vector3(Mathf.Clamp(Body.velocity.x, -MaxSpeed, MaxSpeed) * (1 - ((walkOn == SurfaceType.Ice ? PlayerIceFriction : PlayerFriction) * Time.deltaTime)), Body.velocity.y, 0);
        }
        if (dir.magnitude < 0.1f && Mathf.Abs(Body.velocity.x) < PlayerStopSpeed) Body.velocity = new Vector3(0, Body.velocity.y, 0);
        float modelRotation = Model.localRotation.eulerAngles.y % 360.0f;
        if (modelRotation > 270.0f) modelRotation -= 360.0f;
        if (modelRotation < targetRotation)
        {
            float dec = AngularVelocity* Time.deltaTime;
            Model.Rotate(new Vector3(0, dec, 0));
            modelRotation = Model.localRotation.eulerAngles.y % 360.0f;
            if (modelRotation > 270.0f) modelRotation -= 360.0f;
            if (modelRotation > targetRotation) Model.Rotate(new Vector3(0,targetRotation- modelRotation, 0));
        }
        if (modelRotation > targetRotation)
        {
            float dec = AngularVelocity * Time.deltaTime;
            Model.Rotate(new Vector3(0, -dec, 0));
            modelRotation = Model.localRotation.eulerAngles.y % 360.0f;
            if (modelRotation > 270.0f) modelRotation -= 360.0f;
            if (modelRotation < targetRotation) Model.Rotate(new Vector3(0, targetRotation - modelRotation, 0));
        }
        Vector3 camDelta = (camPosition + targetCamPosition) - MainCamera.transform.localPosition;
        float camRotDelta = (targetCamRotation - FPSCamera.transform.localRotation.eulerAngles.x) % 360.0f;
        if (camRotDelta < -180.0f) camRotDelta += 360.0f;
        if (camDelta.magnitude > CameraDeltaSpeed*Time.deltaTime)
        {
            MainCamera.transform.localPosition += camDelta * Time.deltaTime * CameraDeltaSpeed;
        }
        else
        {
            MainCamera.transform.localPosition = (camPosition + targetCamPosition);
        }
        if (Mathf.Abs(camRotDelta) > CameraDeltaAngularSpeed * Time.deltaTime)
        {
            FPSCamera.transform.Rotate(new Vector3(camRotDelta * Time.deltaTime * CameraDeltaAngularSpeed, 0, 0));
        }
        else
        {
            //FPSCamera.transform.eulerAngles = new Vector3(targetCamRotation, 0, 0);
        }
        if (transform.position.y < DeathFloor) KillEntity();
        Vector3 hVel = Body.velocity;
        hVel.y = 0;
        if (walkOn == SurfaceType.Ice && dir.magnitude < 0.1f) hVel = new Vector3();
        Anim.SetBool("Ground", onGroung);
        Anim.SetFloat("Speed", hVel.magnitude);
    }

    public void OnMove(CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
        if (Mathf.Abs(movementDir.x) < StickDeadZone) movementDir.x = 0.0f;
        if (Mathf.Abs(movementDir.y) < StickDeadZone) movementDir.y = 0.0f;
    }

    public void SetAirBorne()
    {
        onGroung = false;
    }

    public void OnJump(CallbackContext context)
    {
        if (context.started && onGroung && !stunt)
        {
            onGroung = false;
            Body.AddForce(new Vector3(0, JumpPower, 0), ForceMode.Impulse);
        }
    }

    public void OnResetPlayer()
    {
        if (!deathAnim && damageable)
        {
            Health = 0;
            UIHandler.OnHealthChange();
            StartCoroutine(RagdollAnimation());
        }
    }

    public override void KillEntity()
    {
        OnResetPlayer();
    }

    public void DamagePlayer()
    {
        if (!deathAnim && damageable)
        {
            if (Health > 1)
            {
                Health--;
                StartCoroutine(DamageAnimation());
                UIHandler.OnHealthChange();
            }
            else
            {
                KillEntity();
            }
        }
    }

    public void FootCollide(SurfaceType type, bool ground)
    {
        walkOn = type;
        onGroung = ground;
    }

    IEnumerator RagdollAnimation()
    {
        deathAnim = true;
        Ragdoll.ActiveRagdoll();
        yield return WaitFor(2);
        yield return UIHandler.ScreenFadeIn();
        if (Lives == 1)
        {
            UIHandler.OnGameOver();
        }
        else
        {
            Ragdoll.DesactiveRagdoll(RespawnPoint);
            Anim.SetBool("Ground", true);
            Health = MaxHealth;
            Lives--;
            UIHandler.OnHealthChange();
            UIHandler.OnLivesChange();
        }
        yield return UIHandler.ScreenFadeOut();
        deathAnim = false;
        yield return null;
    }

    IEnumerator WaitFor(float timer)
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return Time.deltaTime;
        }
    }

    IEnumerator BlinkFor(float timer, float deltaTime)
    {
        while (timer > 0)
        {
            PlayerRendererSkin.material = DamageMaterial;
            while (timer > 0 && (timer % (2 * deltaTime)) > deltaTime)
            {
                timer -= Time.deltaTime;
                yield return Time.deltaTime;
            }
            PlayerRendererSkin.material = DefaultMaterial;
            while (timer > 0 && (timer % (2 * deltaTime)) <= deltaTime)
            {
                timer -= Time.deltaTime;
                yield return Time.deltaTime;
            }
        }
    }

    IEnumerator DamageAnimation()
    {
        stunt = true;
        Anim.SetBool("Hurt", true);
        damageable = false;
        PlayerRendererSkin.material = DamageMaterial;
        yield return WaitForGround(5, 0.1f);
        yield return BlinkFor(1, 0.1f);
        Anim.SetBool("Hurt", false);
        stunt = false;
        yield return BlinkFor(2, 0.1f);
        PlayerRendererSkin.material = DefaultMaterial;
        damageable = true;
        yield return null;
    }

    IEnumerator WaitForGround(float timer, float deltaTime)
    {
        bool ShouldWaitMore = !onGroung;
        while (timer > 0 && !onGroung)
        {
            PlayerRendererSkin.material = DamageMaterial;
            while (timer > 0 && (timer % (2 * deltaTime)) > deltaTime)
            {
                timer -= Time.deltaTime;
                yield return Time.deltaTime;
            }
            PlayerRendererSkin.material = DefaultMaterial;
            while (timer > 0 && (timer % (2 * deltaTime)) <= deltaTime)
            {
                timer -= Time.deltaTime;
                yield return Time.deltaTime;
            }
        }
        if (ShouldWaitMore)
        {
            Anim.SetBool("Hurt", false);
            yield return BlinkFor(1.0f, 0.1f);
        }
    }
}
