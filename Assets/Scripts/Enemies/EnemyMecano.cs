using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMecano : MonoBehaviour
{
    const int PLAYER_LAYER = 8;

    enum State
    {
        Work, Move, RunAway, Dead
    }

    private static List<Vector3> workPositions = new List<Vector3>();

    public static int nbMecanos = 5;

    [SerializeField] private float deathDuration = 0f;
    [SerializeField] private float maxSight = 15f;

    [Header("Random Work Tools")]
    [SerializeField] private GameObject[] toolsL;
    [SerializeField] private GameObject[] toolsR;
    [SerializeField] private Transform toolTransformL;
    [SerializeField] private Transform toolTransformR;

    [Header("VFX")]
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;

    [Header("Movement")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float targetChangeInterval = 2f;
    private float heureSupps;

    // Animation
    const string MOVE_ANIM = "mecano_move";
    const string WORK_ANIM = "mecano_work";
    const string DIE_ANIM = "mecano_die";
    const string HURT_ANIM = "mecano_hurt";
    const string RUN_ANIM = "mecano_run";
    const string IDLE_ANIM = "mecano_work";

    private GameObject tool;

    private GameObject _playerGameObject;
    private Transform _playerTransform;
    private Health _playerHealth;
    private MultipleViewPlayerController _playerMovement;
    private Vector3 _targetPosition;
    private Health _health;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Collider[] _thisColliders;

    private State _state;
    private float _lastStartToWork = Mathf.NegativeInfinity;
    private float _fleePredictionDelta;
    private float defaultSpeed;

    private string _currentAnim;
    private string _previousAnim;
    private string _nextAnim;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _thisColliders = GetComponentsInChildren<Collider>();

        _playerGameObject = GameObject.FindWithTag("Player");
        _playerTransform = _playerGameObject.transform;
        _playerHealth = _playerGameObject.GetComponent<Health>();
        _playerMovement = _playerGameObject.GetComponent<MultipleViewPlayerController>();

        workPositions.Add(transform.position);
        if (Random.value > 0.35f) Destroy(this.gameObject);
    }

    private void Start()
    {
        _navMeshAgent.avoidancePriority = Random.Range(0, 35);
        _navMeshAgent.speed += Random.value * 0.6f;
        defaultSpeed = _navMeshAgent.speed;

        int r = Random.Range(0, toolsL.Length);
        tool = toolsL[r];
        Instantiate(tool, toolTransformL);
        toolTransformL.localScale /= 15f;

        r = Random.Range(0, toolsR.Length);
        tool = toolsR[r];
        Instantiate(tool, toolTransformR);
        toolTransformR.localScale /= 15f;

        _fleePredictionDelta = Random.value * 2f - 1f;

        StartWork();
    }

    private void ChangeAnimation(string anim)
    {
        if (_currentAnim == anim) return;

        _animator.Play(anim);
        CancelInvoke(nameof(BackToPreviousAnim));
        _previousAnim = _currentAnim;
        _currentAnim = anim;
    }

    private void BackToPreviousAnim()
    {
        ChangeAnimation(_previousAnim);
    }

    private void SetPreviousAnim(string nextAnim)
    {
        _previousAnim = nextAnim;
    }

    public bool IsDead
    {
        get => _health.IsDead;
    }

    private float DistanceToPlayer => Vector3.Distance(transform.position, _playerTransform.position);


    private void Update()
    {
        if (_health.IsDead) return;

        switch (_state)
        {
            case State.Work:
                UpdateWork();
                break;
            case State.Move:
                UpdateMove();
                break;
            case State.RunAway:
                UpdateRunaway();
                break;
            case State.Dead:
                return;
            default:
                break;
        }
    }

    private Vector3 ChooseRandomTarget()
    {
        int randIndex = Random.Range(0, workPositions.Count);
        return workPositions[randIndex];
    }

    private void StartMove()
    {
        _state = State.Move;
        ChangeAnimation(MOVE_ANIM);

        _targetPosition = ChooseRandomTarget();
        _navMeshAgent.SetDestination(_targetPosition);
        _navMeshAgent.speed = defaultSpeed;
    }

    private void StartWork()
    {
        _state = State.Work;
        ChangeAnimation(WORK_ANIM);
        heureSupps = Random.value * 10;
        _lastStartToWork = Time.time;
    }

    private void StartRunaway()
    {
        _state = State.RunAway;
        _navMeshAgent.speed *= 2f;
        ChangeAnimation(RUN_ANIM);
    }

    private void UpdateWork()
    {
        if (Time.time >= _lastStartToWork + targetChangeInterval + heureSupps)
        {
            StartMove();
        }
    }

    private void UpdateMove()
    {
        if (Vector3.Distance(_targetPosition, transform.position) <= _navMeshAgent.stoppingDistance)
        {
            StartWork();
        }
    }

    private void UpdateRunaway()
    {
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + 0.1f)
        {
            ChangeAnimation(IDLE_ANIM);
        }
        else if (_navMeshAgent.remainingDistance < 3f)
        {
            ChangeAnimation(MOVE_ANIM);
        }
        else
        {
            ChangeAnimation(RUN_ANIM);
        }

        Vector3 posNearestSoldier = Soldier.Nearest(transform.position, out float d);
        if (DistanceToPlayer <= d)
        {
            Vector3 playerDirection = _playerTransform.position - transform.position;
            _navMeshAgent.destination = transform.position - playerDirection.normalized * 6f;
        }
        else if (d < maxSight)
        {
            _targetPosition = posNearestSoldier;
            _navMeshAgent.SetDestination(_targetPosition);
        }
    }

    public void OnTakeDamage()
    {
        hitVFX.Play();
        if (_currentAnim != WORK_ANIM && _currentAnim != MOVE_ANIM) return;

        //ChangeAnimation(HURT_ANIM);
        //Invoke(nameof(StartRunaway), _animator.GetCurrentAnimatorStateInfo(0).length);
        StartRunaway();
    }

    // Call by the Health component 
    public void Die()
    {
        _state = State.Dead;
        ChangeAnimation(DIE_ANIM);

        Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject, deathDuration);
        _navMeshAgent.enabled = false;
        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxSight);
        if (_navMeshAgent == null) return;

        Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_navMeshAgent.destination, 0.2f);
    }
}
