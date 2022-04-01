using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{



    [SerializeField]
    public bool _isSelected;

    protected bool _objectActive;


    public virtual void ItemInteraction()
    {
        _objectActive = !_objectActive;
        Debug.Log("Interaction  Input");

        if (_objectActive) ActiveItem();
        if (!_objectActive) DeactiveItem();
    }

    protected virtual void ActiveItem()
    {

        Debug.Log("Item Active");
    }

    protected virtual void DeactiveItem()
    {
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
