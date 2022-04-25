using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainDoor : MonoBehaviour
{
    [SerializeField] bool _isOpen;
    Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Toggle()
    {
        _isOpen = !_isOpen;
        _animator.SetBool("Open", _isOpen);
    }
}
