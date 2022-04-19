using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class WindowTrigger : Trigger
{

    [SerializeField] private string _tagTrigger;
    [SerializeField] private LayerMask _triggerMask;

    [Header("Trigger Setting")]
    [SerializeField]
    private bool _reverse;
    [SerializeField]
    private float _speedOfTransition;

    private float _transitionTimer;
    private bool _isInFront;


    [Header("Camera Setting")]
    [SerializeField] private Vector2 _windowSize;
    [SerializeField] private Vector2 _windowOffset;

    [SerializeField] private Vector2 startWindowSize = new Vector3();
    [SerializeField] private Vector2 startWindowOffset = new Vector3();
    private bool _isTransfom;
    private bool _firstContact = true;
    private BoxCollider _boxCollider;
    private CameraBehavior _cameraBehavior;

    override public void Start()
    {
        base.Start();
        _cameraBehavior = Camera.main.GetComponent<CameraBehavior>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == _tagTrigger)
        {
            if (_firstContact)
            {
                startWindowSize = _cameraBehavior.WindowSize;
                startWindowOffset = _cameraBehavior.WindowOffset;
                _firstContact = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == _tagTrigger)
        {
            Debug.Log(other.gameObject.name);
            bool _testPos = _isInFront;

            if (other.transform.position.x > transform.position.x + _boxCollider.center.x)
                _testPos = true;
            if (other.transform.position.x < transform.position.x + _boxCollider.center.x)
                _testPos = false;

            if (_testPos != _isInFront)
            {
                if (_isTransfom)
                {
                    StopCoroutine(WindowTransition());
                    if (_reverse) ReverseValue();
                    _isTransfom = false;
                }
                StartCoroutine(WindowTransition());
                _isInFront = _testPos;
            }
        }


    }

    private IEnumerator WindowTransition()
    {
        Debug.Log("Collide");

        _isTransfom = true;
        while (_transitionTimer < _speedOfTransition)
        {
            _cameraBehavior.WindowOffset = Vector2.Lerp(startWindowOffset, _windowOffset, _transitionTimer / _speedOfTransition);
            _cameraBehavior.WindowSize = Vector2.Lerp(startWindowSize, _windowSize, _transitionTimer / _speedOfTransition);
            _transitionTimer += Time.deltaTime;
            yield return Time.deltaTime;
        }

        if (_reverse) ReverseValue();
        _transitionTimer = 0;
        _isTransfom = false;

    }

    private void ReverseValue()
    {
        Vector2 _size = _windowSize;
        Vector2 _offset = _windowOffset;
        _windowOffset = startWindowOffset;
        _windowSize = startWindowSize;
        startWindowSize = _size;
        startWindowOffset = _offset;
    }


}
