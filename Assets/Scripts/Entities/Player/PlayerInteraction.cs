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
    [Range(-1f, 1f)]
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
    [SerializeField] public bool _inputReset; // Use for the holding input state

    private Vector2 _axis;
    private PlayerStatus _playerStatus;
    public bool CanStopNow = true; // Used to Lock the player during pushing animation

    public InteractObjects ObjectType { get { return _objectInteractive.ObjectType; } }
    public Vector3 InteractiveObjectPos
    {
        get
        {
            if (_objectInteractive != null) return _objectInteractive.transform.position;
            else return Vector3.zero;
        }
    }
    public Vector3 InteractiveObjectScale
    {
        get
        {
            if (_objectInteractive != null) return _objectInteractive.transform.localScale;
            else return Vector3.zero;
        }
    }
    public InteractiveObject Object { get { return _objectInteractive; } }
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
            if (_objectInteractive.ObjectType == InteractObjects.Ladder)
            {
                _objectInteractive.UpdateItem(_axis);
                return;
            }

            if (_objectManager.IsObjectInRange(transform.position, transform.forward, _detectDistance, _detectionDirection, _objectInteractive))
                _objectInteractive.UpdateItem(_axis);
            else
            {
                _objectInteractive.CancelUpdate();
                UnlinkObject();
            }


        }
        else
        {
            InteractiveObject objectClose = _objectManager.ObjectsInRange(transform.position, transform.forward, _detectDistance, _detectionDirection);
            if (objectClose != null)
            {
                UnselectObject(objectClose);
                if (CanBeSelected(objectClose))
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
        if (_objectManager == null)
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
            _objectInteractive.DeactiveInteraction = true;
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
        if (_objectInteractive != null) _objectInteractive.CancelUpdate();

    }
    #endregion

    #region Selecting Object

    private void ChangeSelectedObject(InteractiveObject interactiveObject)
    {

        _objectInteractive = interactiveObject;
        _objectInteractive.IsSelected = true;
        _uiInteract.SetActive(true);
        _uiInteract.transform.position = _objectInteractive.HintPosition;
        _uiInteract.transform.rotation = _uiRot;
        _uiInteract.transform.SetParent(_objectInteractive.transform);
        _interactionState = InteractionState.Selected;
    }

    private bool CanBeSelected(InteractiveObject interactiveObject)
    {
        if (interactiveObject.transform.position.y >= transform.position.y && (interactiveObject.UseOnlyInShadow && _playerStatus.IsShadow || !interactiveObject.UseOnlyInShadow) && interactiveObject.CanBeSelected)
            return true;
        else
            return false;
    }

    private void UnselectObject()
    {
        if (_objectInteractive)
        {
            _objectInteractive.IsSelected = false;
            _objectInteractive = null;
            _uiInteract.SetActive(false);
            _uiInteract.transform.SetParent(transform);
            _interactionState = InteractionState.None;
        }
    }

    private void UnselectObject(InteractiveObject obj)
    {
        if (_objectInteractive && _objectInteractive != obj)
        {
            _objectInteractive.IsSelected = false;
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

