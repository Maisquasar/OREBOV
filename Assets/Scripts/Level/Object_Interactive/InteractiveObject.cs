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
        Ladder
    }

}

public class InteractiveObject : AmbientTypeHolder
{
    [Header("Object State")]
    [SerializeField] public bool _isSelected;
    [SerializeField] public bool _objectActive = false;
    [SerializeField] public bool _useOnlyInShadow;
    public float ObjectInteractionArea = 0;
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

    [Header("Debug  Settings")]
    [SerializeField] protected bool _debug;

    protected GameObject _playerGO;
    protected Vector2 _axis;
    protected virtual void Start()
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
        _objectActive = true;
    }

    protected virtual void ActiveItem(Enemy enemy)
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundActiveTrigger, transform.position);
        _playerGO = enemy.gameObject;
        _objectActive = true;
    }

    protected virtual void DeactiveItem()
    {
        if (_activeSound)
            AudioSource.PlayClipAtPoint(_soundDeactiveTrigger, transform.position);
        _objectActive = false;
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ObjectInteractionArea);
    }
}
