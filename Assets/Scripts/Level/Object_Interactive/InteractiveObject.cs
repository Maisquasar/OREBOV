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
        LightSwitch,
        Hideout,
        Ladder,
        Stair,
    }

}

public class InteractiveObject : AmbientTypeHolder
{
    [Header("Object State")]
    [HideInInspector] public bool CanBeSelected = true;
    [HideInInspector] public bool IsSelected;
    [HideInInspector] public bool ObjectActive = false;
    public bool UseOnlyInShadow;
    public float ObjectInteractionArea = 0;
    [HideInInspector] public bool DefaultState;
    [HideInInspector] public bool DeactiveInteraction = false;
    [HideInInspector] public InteractObjects ObjectType;

    [Header("UI Postion")]
    [SerializeField] private Vector3 _uiHintPosition;
    public Vector3 HintPosition { get { return transform.position + _uiHintPosition; } }

    [Header("Sound")]
    [SerializeField] protected bool _activeSound = false;
    [SerializeField] protected SoundEffectsHandler _soundActiveTrigger;
    [SerializeField] protected SoundEffectsHandler _soundDeactiveTrigger;

    [Header("Debug  Settings")]
    [SerializeField] protected bool _debug;

    protected GameObject _playerGO;
    protected Vector2 _axis;

    protected virtual void Start()
    {
        DefaultState = ObjectActive;
    }

    public virtual void ItemInteraction(GameObject player)
    {
        ActiveItem(player);
    }

    protected virtual void ActiveItem(GameObject player)
    {
        if (_activeSound && _soundActiveTrigger != null)
            _soundActiveTrigger.PlaySound();
        _playerGO = player;
        ObjectActive = true;
    }

    protected virtual void ActiveItem(Enemy enemy)
    {
        if (_activeSound && _soundActiveTrigger != null)
            _soundActiveTrigger.PlaySound();
        _playerGO = enemy.gameObject;
        ObjectActive = true;
    }

    protected virtual void DeactiveItem()
    {
        if (_activeSound && _soundActiveTrigger != null)
            _soundDeactiveTrigger.PlaySound();
        ObjectActive = false;
    }

    public virtual void UpdateItem(Vector2 axis)
    {
        _axis = axis;
    }

    protected virtual void UpdateItemInternally()
    {
        if (IsSelected) ItemSelected();
    }

    protected virtual void ItemSelected()
    {

    }

    public virtual void HoldUpdate()
    {

    }

    public virtual void CancelUpdate()
    {
        if (ObjectType == InteractObjects.Box || ObjectType == InteractObjects.Hideout)
            DeactiveItem();
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(HintPosition, 0.112f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ObjectInteractionArea);
    }
}
