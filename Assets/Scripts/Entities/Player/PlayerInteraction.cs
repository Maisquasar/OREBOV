using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private float _detectDistance = 1f;

    // ObjectManger Component

    [SerializeField]
    
    private ObjectManager _objectManager;
    // ObjectInteract selected
    [SerializeField]
    private InteractiveObject _objectInteractiveSelected;

    [Header("UI")]
    [SerializeField]
    private GameObject _uiInteract;

    [Header("Debug")]
    [SerializeField]
    private bool _debugActive;

    [SerializeField]
    private bool _inputReset; // Use for the holding the input


    private void Update()
    {

        if (!_inputReset)
            HoldInput();
        if (_objectManager.ObjectsInRange(transform.position, _detectDistance) != null)
            ChangeSelectedObject(_objectManager.ObjectsInRange(transform.position, _detectDistance));
        else
            UnselectObject();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GameMetric.GetUnityValue(_detectDistance));

    }

    #region Input Managing
    public void InteractionInput(InputAction.CallbackContext callback)
    {
        if (callback.started)
            PressInput();

        if (callback.canceled)
            CancelInput();

    }

    private void PressInput()
    {
        if (_debugActive) Debug.Log("Press Input");
        _objectInteractiveSelected.ItemInteraction();
        _inputReset = false;
    }


    private void HoldInput()
    {
        if (_debugActive) Debug.Log("Hold Input");
        if(_objectInteractiveSelected != null)
        {
        _objectInteractiveSelected.UpdateItem();

        }

    }
    private void CancelInput()
    {
        if (_debugActive) Debug.Log("Cancel Input");
        _inputReset = true;
    }
    #endregion

    #region Selecting Object

    private void ChangeSelectedObject(InteractiveObject interactiveObject)
    {
        if(_objectInteractiveSelected != interactiveObject && _objectInteractiveSelected != null)
        {
            UnselectObject();
        }
        _objectInteractiveSelected = interactiveObject;
        _objectInteractiveSelected._isSelected = true;
        _uiInteract.SetActive(true);
        _uiInteract.transform.position = _objectInteractiveSelected.transform.position + Vector3.up * 1f;
    }

    private void UnselectObject()
    {
        _objectInteractiveSelected._isSelected = false;
        _objectInteractiveSelected = null;
        _uiInteract.SetActive(false);
    }

    #endregion

}
