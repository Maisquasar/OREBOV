using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public enum InteractionState
    {
        None,
        Selected,
        Link,
    }

    [SerializeField]
    private float _detectDistance = 1f;

    [SerializeField]
    private InteractionState _interactionState;
    public InteractionState Interaction {get { return _interactionState; }}

    // ObjectManger Component

    [SerializeField]
    private ObjectManager _objectManager;
    // ObjectInteract selected
    [SerializeField]
    private InteractiveObject _objectInteractive;
    [Header("UI")]
    [SerializeField]
    private GameObject _uiInteract;

    [Header("Debug")]
    [SerializeField]
    private bool _debugActive;

    [SerializeField]
    private bool _inputReset; // Use for the holding the input
    public bool CanStopNow = true; // Used to Lock the player during pushing animation

    private Vector2 _axis;

    private void Start()
    {
        if (_objectManager == null)
            _objectManager = new ObjectManager();
    }

    private void Update()
    {
        if (_interactionState == InteractionState.Link)
        {
            _objectInteractive.UpdateItem(_axis);
        }
        else
        {
            if (!_inputReset)
                HoldInput();
            if (_objectManager.ObjectsInRange(transform.position, _detectDistance) != null)
            {
                InteractiveObject objectClose = _objectManager.ObjectsInRange(transform.position, _detectDistance);
                UnselectObject(objectClose);
                ChangeSelectedObject(objectClose);
            }
            else
            {
                UnselectObject();
            }
        }


    }

    private void OnDrawGizmos()
    {
        if (_debugActive)
        {
            Gizmos.DrawWireSphere(transform.position, _detectDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GameMetric.GetUnityValue(_detectDistance));
        }

    }

    #region Input Managing
    public void InteractionInput(InputAction.CallbackContext callback)
    {
        if (callback.started)
            PressInput();

        if (callback.canceled)
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
        _uiInteract.transform.position = _objectInteractive.transform.position + Vector3.up * 1f;
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
}
