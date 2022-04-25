using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainDoor : MonoBehaviour
{
    [SerializeField] bool _isOpen;
    Animator _animator;
    InteractiveStair _stairs;
    // Start is called before the first frame update
    void Start()
    {
        _stairs = GetComponent<InteractiveStair>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Open", _isOpen);
        _stairs.CanBeSelected = _isOpen;
    }

    public void Toggle()
    {
        _isOpen = !_isOpen;
        _stairs.CanBeSelected = _isOpen;
        _animator.SetBool("Open", _isOpen);
    }
}
