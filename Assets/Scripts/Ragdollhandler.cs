using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdollhandler : MonoBehaviour
{
    public GameObject Hips;
    public GameObject LeftUpLeg;
    public GameObject LeftLeg;
    public GameObject RightUpLeg;
    public GameObject RightLeg;
    public GameObject Spine;
    public GameObject LeftShoulder;
    public GameObject LeftArm;
    public GameObject RightShoulder;
    public GameObject RightArm;
    public GameObject Head;

    public GameObject PlayerBody;

    public GameObject mainObject;
    RigidbodyConstraints BodyConstraints;

    public Animator Anim;
    public PlayerController controler;

    GameObject BodiesGroup;
    List<Collider> colliders;
    List<Rigidbody> rigidbodies;
    List<GameObject> ragdollObjects;
    List<Vector3> positions;

    Rigidbody mainRigidBody;
    CapsuleCollider mainCollider;

    // Start is called before the first frame update
    void Start()
    {
        BodiesGroup = new GameObject("BodiesGroup");
        colliders = new List<Collider>();
        rigidbodies = new List<Rigidbody>();
        ragdollObjects = new List<GameObject>();
        positions = new List<Vector3>();
        ragdollObjects.Add(Hips);
        ragdollObjects.Add(LeftUpLeg);
        ragdollObjects.Add(LeftLeg);
        ragdollObjects.Add(RightUpLeg);
        ragdollObjects.Add(RightLeg);
        ragdollObjects.Add(Spine);
        ragdollObjects.Add(LeftShoulder);
        ragdollObjects.Add(LeftArm);
        ragdollObjects.Add(RightShoulder);
        ragdollObjects.Add(RightArm);
        ragdollObjects.Add(Head);
        for (int i = 0; i < ragdollObjects.Count; i++)
        {
            colliders.Add(ragdollObjects[i].GetComponent<Collider>());
            rigidbodies.Add(ragdollObjects[i].GetComponent<Rigidbody>());
            Vector3 tmp = rigidbodies[i].transform.position;
            positions.Add(new Vector3(tmp.x, tmp.y, tmp.z));
        }
        mainRigidBody = mainObject.GetComponent<Rigidbody>();
        mainCollider = mainObject.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveRagdoll()
    {
        controler.enabled = false;
        Anim.enabled = false;
        BodyConstraints = mainRigidBody.constraints;
        Vector3 velocity = mainRigidBody.velocity;
        mainRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        mainCollider.enabled = false;
        for (int i = 0; i < ragdollObjects.Count; i++)
        {
            Vector3 tmp = rigidbodies[i].transform.localPosition;
            positions[i] = new Vector3(tmp.x, tmp.y, tmp.z);
            colliders[i].enabled = true;
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].useGravity = true;
            rigidbodies[i].position = ragdollObjects[i].transform.position;
            rigidbodies[i].constraints = RigidbodyConstraints.None;
            rigidbodies[i].velocity = velocity;
        }
    }

    public void DesactiveRagdoll(Vector3 pos)
    {
        for (int i = 0; i < ragdollObjects.Count; i++)
        {
            colliders[i].enabled = false;
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
        }
        if (PlayerBody != null)
        {
            GameObject copy = Instantiate(PlayerBody, BodiesGroup.transform, true);
            Destroy(copy.GetComponentInChildren(typeof(Camera)).gameObject);
        }
        for (int i = 0; i < ragdollObjects.Count; i++)
        {
            Vector3 tmp = positions[i];
            rigidbodies[i].transform.localPosition = new Vector3(tmp.x, tmp.y, tmp.z);
        }
        mainRigidBody.position = pos;
        mainRigidBody.constraints = BodyConstraints;
        Anim.enabled = true;
        mainCollider.enabled = true;
        controler.enabled = true;
    }
}
