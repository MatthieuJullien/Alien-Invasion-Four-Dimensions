using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{
    enum State
    {
        Patrol, Pursuit, Attak, Dead
    }

    private static List<Transform> soldiers = new List<Transform>();
    public static bool CodeRed = false;

    public static Vector3 Nearest(Vector3 position, out float distance)
    {
        distance = Mathf.Infinity;
        float d;
        Vector3 nearestPos = Vector3.zero;
        foreach (var soldier in soldiers)
        {
            d = Vector3.Distance(position, soldier.position);
            if (d < distance)
            {
                distance = d;
                nearestPos = soldier.position;
            }
        }
        return nearestPos;
    }

    const int PLAYER_LAYER = 8;

    [SerializeField] private float deathDuration = 0f;
    [SerializeField] private float maxSight = 25f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float alertDistance = 18f;
    [SerializeField] private float shootDistance = 14f;
    [SerializeField] private float alertCooldown = 5f;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Movement")]
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float waypointChangeInterval = 2f;
    [SerializeField] private PatrolPath _patrolPath;
    private int currentWaypointIndex = 0;


    const string MOVE_ANIM = "soldier_move";
    const string IDLE_ANIM = "soldier_idle";
    const string DIE_ANIM = "soldier_die";
    const string ATTACK_ANIM = "soldier_attack";
    const string HURT_ANIM = "soldier_hurt";

    private GameObject _playerGameObject;
    private Transform _playerTransform;
    private Health _playerHealth;
    private Vector3 _targetPosition;
    private Health _health;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Collider _collider;

    private State _state = State.Patrol;
    private bool isGuardingWaypoint = true;

    private float lastAlertedTime = Mathf.NegativeInfinity;
    private float _lastAttackTime = Mathf.NegativeInfinity;
    private float _lastTargetChange = Mathf.NegativeInfinity;
    private float _pursuitPredictionStrength;

    private string _currentAnim;
    private string _previousAnim;

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

    private float DistanceToPlayer => Vector3.Distance(transform.position, _playerTransform.position);

    private void Awake()
    {
        if (_patrolPath == null) Debug.LogError("Soldier " + name + "does not have any PatrolPoint assigned.");
        soldiers.Add(transform);

        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

        _playerGameObject = GameObject.FindWithTag("Player");
        _playerTransform = _playerGameObject.transform;
        _playerHealth = _playerGameObject.GetComponent<Health>();
    }

    private void Start()
    {
        _navMeshAgent.avoidancePriority = Random.Range(0, 35);
        _navMeshAgent.speed += (Random.value - 0.5f) / 2f;
        _targetPosition = transform.position;
        _pursuitPredictionStrength = Random.value * 2.5f - 0.75f;
    }

    private void Update()
    {
        if (_health.IsDead) return;

        switch (_state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Pursuit:
                UpdatePursuit();
                break;
            case State.Attak:
                UpdateAttack();
                break;
            case State.Dead:
                return;
            default:
                break;
        }
    }

    private void StartPatrol()
    {
        _state = State.Patrol;
        ChangeAnimation(MOVE_ANIM);
        _navMeshAgent.destination = _patrolPath.GetWaypointPosition(currentWaypointIndex);
    }

    private void StartPursuit()
    {
        _state = State.Pursuit;
        ChangeAnimation(MOVE_ANIM);
        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    private void StartAttack()
    {
        _state = State.Attak;
        _navMeshAgent.SetDestination(transform.position);
    }

    private void UpdatePatrol()
    {
        if (CodeRed)
        {
            Vector3 playerDirection = _playerTransform.position - transform.position;
            if (Physics.Raycast(transform.position, playerDirection, out RaycastHit hit, maxSight))
            {
                if (!_playerHealth.IsDead && hit.collider.gameObject.layer == PLAYER_LAYER)
                {
                    StartPursuit();
                    return;
                }
            }
        }

        if (isGuardingWaypoint)
        {
            if (Time.time >= _lastTargetChange + waypointChangeInterval)
            {
                // time to go next waypoint
                isGuardingWaypoint = false;
                ChangeAnimation(MOVE_ANIM);
                currentWaypointIndex = (currentWaypointIndex + 1) % _patrolPath.Count;
                _targetPosition = _patrolPath.GetWaypointPosition(currentWaypointIndex);
                _navMeshAgent.SetDestination(_targetPosition);
            }
        }
        else if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
        {
            // if at waypoint, wait before going next
            isGuardingWaypoint = true;
            ChangeAnimation(IDLE_ANIM);
            _lastTargetChange = Time.time;
        }
    }

    private void UpdatePursuit()
    {
        if (_playerHealth.IsDead)
        {
            StartPatrol();
            return;
        }

        if ((DistanceToPlayer > maxSight) && (Time.time >= lastAlertedTime + alertCooldown))
        {
            StartPatrol();
        }
        else if (DistanceToPlayer <= shootDistance)
        {
            StartAttack();
        }
        else if (DistanceToPlayer <= _navMeshAgent.stoppingDistance * 1.35f)
        {
            _navMeshAgent.destination = transform.position;
            RotateToward(_navMeshAgent.destination);
        }
        else
        {
            _navMeshAgent.destination = _playerTransform.position;
            /*
            // Attempt to implement pursuit steering behavior:
            float T = DistanceToPlayer / _playerMovement.speed;
            Vector3 newDestination = _playerTransform.position + _playerMovement.movement.velocity * T * _pursuitPredictionStrength;
            _navMeshAgent.destination = newDestination - _navMeshAgent.desiredVelocity;
            RotateToward(_navMeshAgent.destination);
        */
        }
    }

    private void UpdateAttack()
    {
        if (_playerHealth.IsDead)
        {
            StartPatrol();
            return;
        }

        if (DistanceToPlayer > shootDistance) // TODO shoot distance OU nav.stoppingDistance ?
        {
            StartPursuit();
            return;
        }
        RotateToward(_playerTransform.position);
        Attack();
    }

    private void Attack()
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;
        _lastAttackTime = Time.time;

        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.Shoot();
        ChangeAnimation(ATTACK_ANIM);
    }

    private void RotateToward(Vector3 lookTarget)
    {
        Vector3 direction = (lookTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }


    // Call by the Health component 
    public void Die()
    {
        _state = State.Dead;
        ChangeAnimation(DIE_ANIM);
        soldiers.Remove(transform);
        _collider.enabled = false;
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        _navMeshAgent.enabled = false;
        enabled = false;
        Destroy(gameObject, deathDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSight);
        if (_navMeshAgent == null) return;

        Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_navMeshAgent.destination, 0.2f);
    }

    public void OnTakeDamage()
    {
        Alert();
        AlertNearbySoldiers();
        hitVFX.Play();
        if (_currentAnim != IDLE_ANIM && _currentAnim != MOVE_ANIM) return;

        ChangeAnimation(HURT_ANIM);
        Invoke(nameof(BackToPreviousAnim), _animator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void AlertNearbySoldiers()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertDistance, Vector3.up, 0);
        foreach (RaycastHit hit in hits)
        {
            Soldier soldier = hit.collider.GetComponent<Soldier>();
            if (soldier == null) continue;

            soldier.Alert();
        }
    }

    public void Alert()
    {
        lastAlertedTime = Time.time;
        if (_state == State.Dead) return;
        if (_state == State.Pursuit) return;
        StartPursuit();
    }
}
