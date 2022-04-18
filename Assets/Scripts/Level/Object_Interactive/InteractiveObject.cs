using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using InteractObject;


namespace InteractObject
{
    public enum InteractObjects
    {
        Box,
        Switch,
        Hideout,
        OilLight
    }

}

public class InteractiveObject : MonoBehaviour
{
    [Header("Object State")]
    [SerializeField] public bool _isSelected;
    [SerializeField] public bool _objectActive = false;
    [SerializeField] public bool _useOnlyInShadow;
    [HideInInspector] public bool DefaultActive;

    [HideInInspector] public bool _deactiveInteraction = false;

    [Header("UI Postion")]
    [SerializeField] private Vector3 _uiHintPosition;
    
    public Vector3 HintPosition { get { return transform.position + _uiHintPosition; } }
    
    [Header("Sound")]
    [SerializeField] protected bool _activeSound = false;
    [SerializeField] protected AudioClip _soundActiveTrigger;
    [SerializeField] protected AudioClip _soundDeactiveTrigger;
    [HideInInspector] public InteractObjects ObjectType;

    [Header("Debug")]
    [SerializeField] protected bool _debug;

    protected GameObject _playerGO;
    protected Vector2 _axis;
    private void Start()
    {
        DefaultActive = _objectActive;
    }


    public virtual void ItemInteraction(GameObject player)
    {
        ActiveItem(player);
    }

    protected virtual void ActiveItem(GameObject player)
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundActiveTrigger, transform.position);
        _playerGO = player;
    }

    protected virtual void ActiveItem(Enemy enemy)
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundActiveTrigger, transform.position);
        _playerGO = enemy.gameObject;
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

    public virtual void CancelUpdate()
    {
        DeactiveItem();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(HintPosition, 0.112f);
    }
}
