using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class MobileEnemy : Enemy
{
    [Header("Controller")]
    [SerializeField] public MobileEnemyMovement _controller;


    [Tooltip("The time the enemy search the player after follow him")]
    [SerializeField] float SearchTime = 1f;

    [SerializeField] private float _detectDistance = 5f;
    [Range(-1f, 1f)]
    [SerializeField] private float _detectionDirection = -1f;

    [SerializeField] private bool _followPlayerOnDetection = true;

    ObjectManager _objectManager;
    EnemyCheckpointManager _checkpointManager;
    int _currentCheckpoint;

    float _timeStamp;
    float _secondCheckStuck = 5f;
    Vector3 _precPoS;

    public override EntityMovement Controller { get { return _controller; } }

    override public void Start()
    {
        if (_controller == null)
            _controller = GetComponent<MobileEnemyMovement>();
        _entityController = _controller;
        _objectManager = FindObjectOfType<ObjectManager>();
        _currentCheckpoint = 0;
        StartCoroutine(WaitStart());
        State = EnemyState.NORMAL;
    }

    IEnumerator WaitStart()
    {
        yield return new WaitUntil(() => _checkpointManager != null);
        base.Start();
    }

    public override void Shoot()
    {
        var lookPos = _player.transform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        transform.rotation *= Quaternion.Euler(0, 90, 0);
        _controller.animator.SetBool("Shooting", true);
    }

    protected override void EndShooting()
    {
        base.EndShooting();
        _controller.animator.SetBool("Shooting", false);
    }

    public void SetCheckpointManager(EnemyCheckpointManager manage)
    {
        _checkpointManager = manage;
    }

    bool wasChange = false;
    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        // If Position to go equals zero then change checkpoint.
        if (_controller.GoTo == Vector3.zero)
        {
            CheckpointChange();
        }
        if (_player != null && _player.Dead)
            return;
        // If Missing Checkpoint Manager.
        if (_checkpointManager == null)
        {
            Debug.LogError("Missing Checkpoint Manager");
            return;
        }

        if (State == EnemyState.CHASE || State == EnemyState.SUSPICIOUS || State == EnemyState.INTERACT)
            _controller.animator.SetBool("Chase", true);
        else
            _controller.animator.SetBool("Chase", false);

        if (!stillWaiting)
        {
            if ((_followPlayerOnDetection && State != EnemyState.SUSPICIOUS) || !_followPlayerOnDetection)
                if ((int)transform.position.x != (int)_checkpointManager.Checkpoints[_currentCheckpoint].transform.position.x || State == EnemyState.CHASE)
                    _controller.Move(_controller.Direction);
            if (State != EnemyState.CHASE && (int)transform.position.x == (int)_checkpointManager.Checkpoints[_currentCheckpoint].transform.position.x && !WaitForToggle)
            {
                CheckpointChange();
            }
            // If Time = -1
            else if (WaitForToggle && State != EnemyState.CHASE && (int)transform.position.x == (int)_checkpointManager.Checkpoints[_currentCheckpoint].transform.position.x && !_hasRotated)
            {
                Rotate();
            }
            else if (State == EnemyState.CHASE && (int)transform.position.x == (int)lastPlayerPos.x)
            {
                StopFollowingPlayer();
            }
        }

        CheckForInteraction();
        CheckIfStuck();
    }

    private void CheckForInteraction()
    {
        // Get The Closest Object.
        InteractiveObject objectClose = _objectManager.ObjectsInRange(transform.position, transform.forward * -1, _detectDistance, _detectionDirection);
        // If object is a LightSwitch
        if (objectClose != null && objectClose.ObjectType == InteractObject.InteractObjects.LightSwitch)
        {
            // Check if the state of the switch is different from the initial state.
            // Check also if the state was changed between the first detection and the actual (case when player switch 2 times).
            // If Enemy is Chasing then continue.
            if (objectClose.DefaultState != objectClose.ObjectActive && State != EnemyState.CHASE || wasChange && State != EnemyState.CHASE)
            {
                wasChange = true;
                State = EnemyState.INTERACT;
                // Set the New Checkpoint to switch position.
                _controller.NewCheckpoint(new Vector3(objectClose.transform.position.x, transform.position.y, transform.position.z));
                if ((int)transform.position.x == (int)objectClose.transform.position.x)
                {
                    // Interact with Item and Wait 2 seconds.
                    wasChange = false;
                    if (objectClose.DefaultState != objectClose.ObjectActive)
                        objectClose.ItemInteraction(this.gameObject);
                    StartCoroutine(WaitCheckpoint(2, _checkpointManager.Checkpoints[_currentCheckpoint].transform.position));
                    State = EnemyState.NORMAL;
                }
            }
            else if (State == EnemyState.INTERACT)
            {
                State = EnemyState.NORMAL;
            }
        }
    }

    private void CheckIfStuck()
    {
        // Check if same position every {_secondCheckStuck} in seconds.
        if (_timeStamp <= Time.time && _checkpointManager.Checkpoints[_currentCheckpoint].Time != -1)
        {
            // If same position and chase : stop following player and go to next checkpoint.
            if (_precPoS == (Vector3)transform.position && State == EnemyState.CHASE)
            {
                StopFollowingPlayer();
            }
            // If same position : go to next checkpoint.
            else if (_precPoS == (Vector3)transform.position && State != EnemyState.CHASE)
            {
                CheckpointChange();
            }
            //Set the precedent position.
            _precPoS = transform.position;
            _timeStamp = Time.time + _secondCheckStuck;
        }
    }

    bool _hasRotated = false;
    void Rotate()
    {
        _hasRotated = true;
        StartCoroutine(LerpFromTo(transform.rotation, transform.rotation * Quaternion.Euler(0, _checkpointManager.Checkpoints[_currentCheckpoint].Angle, 0), 0.2f));
    }

    void StopFollowingPlayer()
    {
        State = EnemyState.NORMAL;
        StartCoroutine(WaitPlayerSearch());
    }

    Vector3 lastPlayerPos;
    override public void GoToPlayer(Vector3 lastPlayerPos)
    {
        stillWaiting = false;
        this.lastPlayerPos = lastPlayerPos;
        State = EnemyState.CHASE;
        _controller.NewCheckpoint(lastPlayerPos);
    }

    bool decrease = false;
    int _precCheckpoint;
    public void CheckpointChange()
    {
        _hasRotated = false;
        StartCoroutine(WaitCheckpoint());
        if (!_checkpointManager.Reverse)
        {
            if (_currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                _currentCheckpoint++;
            else
                _currentCheckpoint = 0;
        }
        else
        {
            if (decrease)
            {
                if (_currentCheckpoint > 0)
                {
                    _currentCheckpoint--;
                }
                else
                {
                    _currentCheckpoint++;
                    decrease = false;
                }
            }
            else
            {
                if (_currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                {
                    _currentCheckpoint++;
                }
                else
                {
                    _currentCheckpoint--;
                    decrease = true;
                }
            }
        }
        if (_checkpointManager.Checkpoints[_currentCheckpoint].Time == -1)
        {
            WaitForToggle = true;
        }
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
    }

    bool WaitForToggle = false;
    public void GoToNextCheckpoint()
    {
        WaitForToggle = false;
        stillWaiting = false;
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
    }

    IEnumerator WaitPlayerSearch()
    {
        stillWaiting = true;
        yield return new WaitForSeconds(SearchTime);
        if (_checkpointManager.Checkpoints[_precCheckpoint].Time == -1)
            _currentCheckpoint = _precCheckpoint;
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
        State = EnemyState.NORMAL;
        stillWaiting = false;
    }


    IEnumerator WaitCheckpoint(float time, Vector3 newPos)
    {
        int indexAtStart = _currentCheckpoint;
        stillWaiting = true;
        yield return new WaitForSeconds(time);
        stillWaiting = false;
        _controller.NewCheckpoint(newPos);
    }

    IEnumerator LerpFromTo(Quaternion initial, Quaternion goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.rotation = goTo;
    }

    bool stillWaiting = false;
    IEnumerator WaitCheckpoint()
    {
        int indexAtStart = _currentCheckpoint;
        stillWaiting = true;
        StartCoroutine(LerpFromTo(transform.rotation, transform.rotation * Quaternion.Euler(0, _checkpointManager.Checkpoints[indexAtStart].Angle, 0), _checkpointManager.Checkpoints[indexAtStart].Time * 25 / 100));
        yield return new WaitForSeconds(_checkpointManager.Checkpoints[indexAtStart].Time);
        stillWaiting = false;
    }
}
