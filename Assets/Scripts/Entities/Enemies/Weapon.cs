using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    
    private Animator _animator;
    private ParticleSystem _particle;
    private SoundEffectsHandler _shootEffect;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _particle = GetComponentInChildren<ParticleSystem>();
        _shootEffect = GetComponent<SoundEffectsHandler>();
    }

    virtual public void Shoot()
    {
        _particle.Play();
        _shootEffect.PlaySound();
    }
}
