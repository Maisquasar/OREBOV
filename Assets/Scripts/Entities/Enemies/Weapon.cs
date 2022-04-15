using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Animator _animator;
    ParticleSystem _particle;
    [SerializeField] protected float _cooldown;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _particle = GetComponentInChildren<ParticleSystem>();
    }

    virtual public void Shoot()
    {
        _particle.Play();
    }
}
