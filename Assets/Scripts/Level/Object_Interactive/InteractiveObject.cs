using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class InteractiveObject : MonoBehaviour
{


    [Header("Object State")]
    [SerializeField]
    public bool _isSelected;
    [SerializeField]    
    public bool _objectActive;

    [Header("Sound")]
    [SerializeField]
    protected bool _activeSound = false;
    [SerializeField]
    protected AudioClip _soundActiveTrigger;
    [SerializeField]
    protected AudioClip _soundDeactiveTrigger;

    [Header("Debug")]
    [SerializeField]
    protected bool _debug;


    protected GameObject _playerGO;
    protected Vector2 _axis;


    public virtual void ItemInteraction(GameObject player)
    {
        _objectActive = !_objectActive;
        
        if (_objectActive)
        {
            ActiveItem(player);
            return;
        }
        if (!_objectActive)
        {
            DeactiveItem();
            return;
        }
    }



    protected virtual void ActiveItem(GameObject player)
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundActiveTrigger, transform.position);

        _playerGO = player;
    }

    protected virtual void DeactiveItem()
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundDeactiveTrigger, transform.position);
    }

    public virtual void UpdateItem(Vector2 axis)
    {
        _axis = axis;
    }

    protected virtual void UpdateItemInternally()
    {
        if (_isSelected) ItemSelected();
    }

    protected virtual void ItemSelected()
    {

    }

    public virtual void HoldUpdate()
    {

    }




}
