using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class Alien : MonoBehaviour
{
    const int PLAYER_LAYER = 8;

    enum AlienState
    {
        Respawn, Patrol, Pursuit, Attak, Dead
    }

    [SerializeField] private float deathDuration = 0f;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private Alien alienPrefab;
    [SerializeField] private float maxSight = 30f;

    [Header("Render")]
    [SerializeField] private Renderer alienRenderer;
    [SerializeField] private Material respawnMaterial;
    [SerializeField] private Material activeMaterial;


    [Header("Duplication")]
    [SerializeField] private float duplicationInterval = 10f;
    [SerializeField] private int maxDuplication = 2;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamage = 20f;

    [Header("Movement")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float targetChangeInterval = 2f;
    [SerializeField] private float targetChangeDistance = 6f;

    private static readonly int DieAnim = Animator.StringToHash("Die");
    private static readonly int IdleAnim = Animator.StringToHash("Eat_Cycle_1");
    private static readonly int MoveAnim = Animator.StringToHash("Walk_Cycle_2");
    private static readonly int AttackAnim = Animator.StringToHash("Attack_4");

    private int alienLayerMask;

    private GameObject _playerGameObject;
    private Transform _playerTransform;
    private Health _playerHealth;
    private MultipleViewPlayerController _playerMovement;
    private Vector3 _targetPosition;
    private Health _health;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Collider[] _thisColliders;

    private AlienState _state = AlienState.Respawn;
    private float _duplicationTimer;
    private int _duplicationCount = 0;
    private float _lastAttackTime = Mathf.NegativeInfinity;
    private float _lastTargetChange = Mathf.NegativeInfinity;
    private float _nbMaxColliderToSpawn;
    private bool _isIdle = false;
    private float _pursuitPredictionStrenght;

    public bool IsDead
    {
        get => _health.IsDead;
    }

    private float DistanceToPlayer => Vector3.Distance(transform.position, _playerTransform.position);

    private void Awake()
    {
        alienLayerMask = LayerMask.GetMask("Alien");

        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _thisColliders = GetComponentsInChildren<Collider>();

        _playerGameObject = GameObject.FindWithTag("Player");
        _playerTransform = _playerGameObject.transform;
        _playerHealth = _playerGameObject.GetComponent<Health>();
        _playerMovement = _playerGameObject.GetComponent<MultipleViewPlayerController>();
    }

    private void Start()
    {
        _duplicationTimer = duplicationInterval;
        _targetPosition = transform.position;
        alienRenderer.material = respawnMaterial;
        _nbMaxColliderToSpawn = _thisColliders.Length;
        _pursuitPredictionStrenght = Random.value;
    }

    private void Update()
    {
        if (_health.IsDead) return;

        if (_duplicationCount < maxDuplication)
        {
            TryDuplicate();
        }

        switch (_state)
        {
            case AlienState.Respawn:
                UpdateRespawn();
                break;
            case AlienState.Patrol:
                UpdatePatrol();
                break;
            case AlienState.Pursuit:
                UpdatePursuit();
                break;
            case AlienState.Attak:
                UpdateAttack();
                break;
            case AlienState.Dead:
                return;
            default:
                break;
        }
    }

    private void UpdateRespawn()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, alienLayerMask);
        if (colliders.Length <= Mathf.FloorToInt(_nbMaxColliderToSpawn))
        {
            foreach (var collider in _thisColliders)
            {
                collider.enabled = true;
            }
            alienRenderer.material = activeMaterial;
            StartPatrol();
        }
        else
        {
            _nbMaxColliderToSpawn += 0.3f * Time.deltaTime;
        }
    }

    private Vector3 ChooseRandomTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * targetChangeDistance;
        if (Vector3.Angle(transform.forward, randomDirection) > 90)
            return ChooseRandomTarget();

        randomDirection += transform.position;
        return randomDirection;
    }

    private void StartPatrol()
    {
        _state = AlienState.Patrol;
        _animator.SetTrigger(MoveAnim);
        _navMeshAgent.destination = ChooseRandomTarget(); ;
    }

    private void StartPursuit()
    {
        _state = AlienState.Pursuit;
        _animator.SetTrigger(MoveAnim);
        _navMeshAgent.destination = _playerTransform.position;
    }

    private void StartAttack()
    {
        _state = AlienState.Attak;
    }

    private void UpdatePatrol()
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

        if (_navMeshAgent.velocity.magnitude < 1f && !_isIdle)
        {
            _animator.SetTrigger(IdleAnim);
            _isIdle = true;
        }
        else if (_navMeshAgent.velocity.magnitude >= 1f && _isIdle)
        {
            _animator.SetTrigger(MoveAnim);
            _isIdle = false;
        }

        if (Time.time >= _lastTargetChange + targetChangeInterval)
        {
            _targetPosition = ChooseRandomTarget();
            _navMeshAgent.SetDestination(_targetPosition);
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

        if (DistanceToPlayer > maxSight)
        {
            StartPatrol();
        }
        else if (DistanceToPlayer <= _navMeshAgent.stoppingDistance)
        {
            StartAttack();
        }
        else
        {

            //
            // T = distancetoplayer / playerMaxsSpeed
            // newDestination  = playerPosition + playerVelocity * T
            //
            // seek = newdestination - currentDestination
            //
            //
            // Attempt to implement pursuit steering behavior:
            float T = DistanceToPlayer / _playerMovement.speed;
            Vector3 newDestination = _playerTransform.position + _playerMovement.movement.velocity * T * _pursuitPredictionStrenght;
            _navMeshAgent.destination = newDestination - _navMeshAgent.desiredVelocity;
            RotateTowardDestination();
        }
    }

    private void UpdateAttack()
    {
        if (_playerHealth.IsDead)
        {
            StartPatrol();
            return;
        }

        if (DistanceToPlayer > _navMeshAgent.stoppingDistance)
        {
            StartPursuit();
            return;
        }

        Attack();
    }

    private void TryDuplicate()
    {
        _duplicationTimer -= Time.deltaTime;
        if (_duplicationTimer > 0) return;

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 1f;
        Alien newAlien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity);
        newAlien.maxDuplication--;
        _duplicationTimer = duplicationInterval + Random.value;
        _duplicationCount++;
    }

    private void RotateTowardDestination()
    {
        Vector3 direction = (_navMeshAgent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void Attack()
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;

        _animator.SetTrigger(AttackAnim);

        _lastAttackTime = Time.time;
        _playerHealth.TakeDamage(attackDamage);
    }

    // Call by the Health component 
    public void Die()
    {
        _state = AlienState.Dead;
        _animator.SetTrigger(DieAnim);

        foreach (var collider in _thisColliders)
        {
            collider.enabled = false;
        }
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject, deathDuration);
        _navMeshAgent.enabled = false;
        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSight);
        if (_navMeshAgent == null) return;

        Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_navMeshAgent.destination, 1f);
    }
}