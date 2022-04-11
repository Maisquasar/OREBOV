using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using InteractObject;

public class PlayerInteraction : MonoBehaviour
{
    public enum InteractionState
    {
        None,
        Selected,
        Link,
    }

    [SerializeField] private float _detectDistance = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _detectionDirection = 0.1f;

    [SerializeField] private InteractionState _interactionState;

    [Header("Object Manager")]
    [SerializeField] private ObjectManager _objectManager;
    [SerializeField] private InteractiveObject _objectInteractive; // ObjectInteract selected

    [Header("UI")]
    [SerializeField] private GameObject _uiInteract;
    private Quaternion _uiRot;

    [Header("Debug")]
    [SerializeField] private bool _debugActive;
    [SerializeField] public bool _inputReset; // Use for the holding the input

    private Vector2 _axis;
    private PlayerStatus _playerStatus;
    public bool CanStopNow = true; // Used to Lock the player during pushing animation

    public InteractObjects ObjectType { get { return _objectInteractive.ObjectType; } }
    public Vector3 InteractiveObjectPos { get { return _objectInteractive.transform.position; } }
    public Vector3 InteractiveObjectScale { get { return _objectInteractive.transform.localScale; } }
    public InteractiveObject Object { get { return _objectInteractive != null ? _objectInteractive : null; } }
    public InteractionState Interaction { get { return _interactionState; } }

    private void Start()
    {
        _playerStatus = GetComponent<PlayerStatus>();
        _uiRot = _uiInteract.transform.rotation;
    }

    private void Update()
    {
        if (!_inputReset)
            HoldInput();

        if (_interactionState == InteractionState.Link)
        {
            _objectInteractive.UpdateItem(_axis);
        }
        else
        {

            InteractiveObject objectClose = _objectManager.ObjectsInRange(transform.position, transform.forward, _detectDistance, _detectionDirection);
            if (objectClose != null)
            {
                    UnselectObject(objectClose);
                if (objectClose._useOnlyInShadow && _playerStatus.IsShadow || !objectClose._useOnlyInShadow)
                {
                    ChangeSelectedObject(objectClose);
                }
            }
            else
            {
                UnselectObject();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(_objectManager == null)
        {
            Debug.LogError("Object Manager is missing in Player Interaction");
        }

        if (_debugActive)
        {
            Gizmos.DrawWireSphere(transform.position, _detectDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GameMetric.GetUnityValue(_detectDistance));
        }
    }

    #region Input Managing
    public void InteractionInput(bool started, bool canceled)
    {
        if (started)
            PressInput();

        if (canceled)
            CancelInput();

    }

    public void AxisInput(InputAction.CallbackContext callback)
    {
        if (callback.performed)
            _axis = callback.ReadValue<Vector2>();
        if (callback.canceled)
            _axis = Vector2.zero;
    }

    private void PressInput()
    {
        if (_objectInteractive != null && CanStopNow)
        {
            _inputReset = false;
            _objectInteractive.ItemInteraction(gameObject);
        }
        if (!CanStopNow)
        {
            _objectInteractive._deactiveInteraction = true;
        }
    }


    private void HoldInput()
    {
        if (_objectInteractive != null)
        {
            _objectInteractive.HoldUpdate();

        }

    }
    private void CancelInput()
    {
        if (CanStopNow) _inputReset = true;

    }
    #endregion

    #region Selecting Object

    private void ChangeSelectedObject(InteractiveObject interactiveObject)
    {

        _objectInteractive = interactiveObject;
        _objectInteractive._isSelected = true;
        _uiInteract.SetActive(true);
        _uiInteract.transform.position = _objectInteractive.HintPosition;
        _uiInteract.transform.rotation = _uiRot;
        _interactionState = InteractionState.Selected;
    }

    private void UnselectObject()
    {
        if (_objectInteractive)
        {
            _objectInteractive._isSelected = false;
            _objectInteractive = null;
            _uiInteract.SetActive(false);
            _interactionState = InteractionState.None;
        }
    }

    private void UnselectObject(InteractiveObject obj)
    {
        if (_objectInteractive && _objectInteractive != obj)
        {
            _objectInteractive._isSelected = false;
            _objectInteractive = null;
            _uiInteract.SetActive(false);
        }
    }

    #endregion

    #region Link Object
    public void LinkObject(InteractiveObject objectToLink)
    {
        UnselectObject();
        _interactionState = InteractionState.Link;
        _objectInteractive = objectToLink;
    }

    public void UnlinkObject()
    {
        UnselectObject();
        _interactionState = InteractionState.None;
    }

    #endregion 
}

