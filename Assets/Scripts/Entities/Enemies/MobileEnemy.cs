using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class MobileEnemy : Enemy
{
    [Header("Controller")]
    [HideInInspector] public MobileEnemyMovement _controller;


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
    float _secondCheckStuck = 2f;
    Vector3 _precPoS;

    public override EntityMovement Controller { get { return _controller; } }

    override public void Start()
    {
        _controller = GetComponent<MobileEnemyMovement>();
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

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        if (_player != null && _player.Dead)
            return;
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
            if (_followPlayerOnDetection && State != EnemyState.SUSPICIOUS || !_followPlayerOnDetection)
                _controller.Move(_controller.Direction);
            if (State != EnemyState.CHASE && (int)transform.position.x == (int)_checkpointManager.Checkpoints[_currentCheckpoint].transform.position.x)
            {
                CheckpointChange();
            }
            else if (State == EnemyState.CHASE && (int)transform.position.x == (int)lastPlayerPos.x)
            {
                StopFollowingPlayer();
            }
        }

        // Enemy Interaction
        InteractiveObject objectClose = _objectManager.ObjectsInRange(transform.position, transform.forward * -1, _detectDistance, _detectionDirection);
        if (objectClose != null)
        {
            if (objectClose.DefaultActive != objectClose._objectActive && State != EnemyState.CHASE)
            {
                State = EnemyState.INTERACT;
                _controller.NewCheckpoint(new Vector3(objectClose.transform.position.x, transform.position.y, transform.position.z));
                if ((int)transform.position.x == (int)objectClose.transform.position.x)
                {
                    objectClose.ItemInteraction(this.gameObject);
                    StartCoroutine(WaitCheckpoint(2, _checkpointManager.Checkpoints[_currentCheckpoint].transform.position));
                    State = EnemyState.NORMAL;
                }
            }
        }

        // Check if same position every {_secondCheckStuck} in seconds.
        if (_timeStamp <= Time.time)
        {
            if (_precPoS == (Vector3)transform.position && State == EnemyState.CHASE)
            {
                StopFollowingPlayer();
            }
            else if (_precPoS == (Vector3)transform.position && State != EnemyState.CHASE)
            {
                CheckpointChange();
            }
            _precPoS = transform.position;
            _timeStamp = Time.time + _secondCheckStuck;
        }
    }


    void StopFollowingPlayer()
    {
        Debug.Log("Stop following Player");
        State = EnemyState.NORMAL;
        StartCoroutine(WaitPlayerSearch());
    }

    Vector3 lastPlayerPos;
    override public void GoToPlayer(Vector3 lastPlayerPos)
    {
        this.lastPlayerPos = lastPlayerPos;
        Debug.Log("Follow Player");
        State = EnemyState.CHASE;
        _controller.NewCheckpoint(lastPlayerPos);
    }

    bool decrease = false;
    public void CheckpointChange()
    {
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
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
    }

    IEnumerator WaitPlayerSearch()
    {
        stillWaiting = true;
        yield return new WaitForSeconds(SearchTime);
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
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
