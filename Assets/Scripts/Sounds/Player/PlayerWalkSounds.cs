using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class PlayerWalkSounds : MonoBehaviour
{
    [Header("Sounds ")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _walkSoundFrequency = 0.4f;

    private EntityMovement _mouvement;
    private bool _canTriggerSounds = true;

    #region InitScript

    private void Start()
    {
        InitComponents();
    }

    private void InitComponents()
    {
        _playerMouvement = GetComponentInParent<PlayerMovement>();
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (IsSoundPlayable(other))
        {
            if (_mouvement.WalkSoundManager())
            {
                _canTriggerSounds = false;
                StartCoroutine(ResetSound());
            }
        }
    }



    private bool IsSoundPlayable(Collider other)
    {
        if (ToolsBox.IsInLayerMask(other.gameObject, _groundLayer) && _canTriggerSounds)
            return true;
        else
            return false;

    }

    private IEnumerator ResetSound()
    {
        yield return new WaitForSeconds(_walkSoundFrequency);
        _canTriggerSounds = true;
    }

}
