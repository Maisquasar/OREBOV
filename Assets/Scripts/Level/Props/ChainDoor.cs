using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainDoor : MonoBehaviour
{
    [SerializeField] bool _isOpen;
    Animator _animator;
    InteractiveStair[] _stairs;
    // Start is called before the first frame update
    void Start()
    {
        GetStairs();
        _animator = GetComponent<Animator>();
        _animator.SetBool("Open", _isOpen);
        SetStairs(_isOpen);
    }

    void GetStairs()
    {
        _stairs = GetComponentsInChildren<InteractiveStair>();
    }

    void SetStairs(bool value)
    {
        foreach (var stair in _stairs)
            stair.CanBeSelected = value;

    }

    public void Toggle()
    {
        _isOpen = !_isOpen;
        SetStairs(_isOpen);
        _animator.SetBool("Open", _isOpen);
    }
}
