using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Animator _animator;
    [SerializeField] protected float _cooldown;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    virtual public void Shoot()
    {
        if (_animator != null)
            _animator.SetTrigger("Shoot");
    }
}
