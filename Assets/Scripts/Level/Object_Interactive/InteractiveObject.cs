using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class InteractiveObject : MonoBehaviour
{



    [SerializeField]
    public bool _isSelected;
    protected bool _objectActive;

    [Header("Sound")]
    [SerializeField]
    protected bool _activeSound = false;
    [SerializeField]
    protected AudioClip _soundActiveTrigger;
    [SerializeField]
    protected AudioClip _soundDeactiveTrigger;



    public virtual void ItemInteraction()
    {
        _objectActive = !_objectActive;
        Debug.Log("Interaction  Input");

        if (_objectActive) ActiveItem();
        if (!_objectActive) DeactiveItem();
    }

    protected virtual void ActiveItem()
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundActiveTrigger, transform.position);
        Debug.Log("Item Active");
    }

    protected virtual void DeactiveItem()
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundDeactiveTrigger, transform.position);
        Debug.Log("Item Deactive");
    }

    public virtual void UpdateItem()
    {
        Debug.Log("Item Update");
    }

    protected virtual void UpdateItemInternally()
    {
        if (_isSelected) ItemSelected();
        Debug.Log("Item Update");
    }

    protected virtual void ItemSelected()
    {

    }




}
