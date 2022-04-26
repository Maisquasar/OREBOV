using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimActivation : MonoBehaviour
{
    [SerializeField] private string _animNameTrigger;
    [SerializeField] private Animator _entityAnimator;
    private bool _playOnce;

    public void ActivateAnim()
    {
        _entityAnimator.SetBool(_animNameTrigger, true);
    }
}
