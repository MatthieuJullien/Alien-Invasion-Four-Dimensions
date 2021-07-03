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
    [SerializeField] private Alien alienPrefab;
    [SerializeField] private float maxSight = 30f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;

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

    // TODO randomize animations?
    const string MOVE_ANIM = "alien_move";
    const string IDLE_ANIM = "alien_idle";
    const string DIE_ANIM = "alien_die";
    const string ATTACK_ANIM = "alien_attack";
    const string HURT_ANIM = "alien_hurt";


    private int alienLayerMask;

    private GameObject _playerGameObject;
    private Transform _playerTransform;
    private Health _playerHealth;
    private MultipleViewPlayerController _playerMovement;
    private Vector3 _targetPosition;
    private Health _health;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private NavMeshObstacle _navMeshObstacle;
    private Collider[] _thisColliders;

    private AlienState _state = AlienState.Respawn;
    private float _duplicationTimer;
    private int _duplicationCount = 0;
    private float _lastAttackTime = Mathf.NegativeInfinity;
    private float _lastTargetChange = Mathf.NegativeInfinity;
    private float _nbMaxColliderToSpawn;
    private bool _isIdle = false;
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
        _navMeshObstacle = GetComponent<NavMeshObstacle>();
        _thisColliders = GetComponentsInChildren<Collider>();

        _playerGameObject = GameObject.FindWithTag("Player");
        _playerTransform = _playerGameObject.transform;
        _playerHealth = _playerGameObject.GetComponent<Health>();
        _playerMovement = _playerGameObject.GetComponent<MultipleViewPlayerController>();

    }

    private void Start()
    {
        _navMeshObstacle.enabled = false;
        _navMeshAgent.avoidancePriority = Random.Range(0, 35);
        _navMeshAgent.speed += (Random.value - 0.5f) / 2f;
        _duplicationTimer = duplicationInterval;
        _targetPosition = transform.position;
        alienRenderer.material = respawnMaterial;
        _nbMaxColliderToSpawn = _thisColliders.Length;
        _pursuitPredictionStrength = Random.value * 2.5f - 0.75f;
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
        _navMeshObstacle.enabled = false;
        _navMeshAgent.enabled = true;
        ChangeAnimation(MOVE_ANIM);
        _navMeshAgent.destination = ChooseRandomTarget();
    }

    private void StartPursuit()
    {
        _state = AlienState.Pursuit;
        _navMeshObstacle.enabled = false;
        _navMeshAgent.enabled = true;
        ChangeAnimation(MOVE_ANIM);
        _navMeshAgent.destination = _playerTransform.position;
    }

    private void StartAttack()
    {
        _state = AlienState.Attak;
        _navMeshAgent.SetDestination(transform.position);
        _navMeshObstacle.enabled = true;
        _navMeshAgent.enabled = false;
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
            ChangeAnimation(IDLE_ANIM);
            _isIdle = true;
        }
        else if (_navMeshAgent.velocity.magnitude >= 1f && _isIdle)
        {
            ChangeAnimation(MOVE_ANIM);
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
        else if (DistanceToPlayer <= _navMeshAgent.stoppingDistance * 1.35f)
        {
            _navMeshAgent.destination = _playerTransform.position;
            RotateToward(_navMeshAgent.destination);
        }
        else
        {
            // Attempt to implement pursuit steering behavior:
            float T = DistanceToPlayer / _playerMovement.speed;
            Vector3 newDestination = _playerTransform.position + _playerMovement.movement.velocity * T * _pursuitPredictionStrength;
            _navMeshAgent.destination = newDestination - _navMeshAgent.desiredVelocity;
            RotateToward(_navMeshAgent.destination);
        }
    }

    private void UpdateAttack()
    {
        if (_playerHealth.IsDead)
        {
            StartPatrol();
            return;
        }

        if (DistanceToPlayer > _navMeshAgent.stoppingDistance + 0.2f)
        {
            StartPursuit();
            return;
        }
        RotateToward(_playerTransform.position);
        Attack();
    }

    private void TryDuplicate()
    {
        _duplicationTimer -= Time.deltaTime;
        if (_duplicationTimer > 0) return;

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 0.5f;
        Alien newAlien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity);
        newAlien.maxDuplication--;
        _duplicationTimer = duplicationInterval + Random.value;
        _duplicationCount++;
    }

    private void RotateToward(Vector3 lookTarget)
    {
        Vector3 direction = (lookTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    private void Attack()
    {
        if (Time.time < _lastAttackTime + attackCooldown) return;
        _lastAttackTime = Time.time;
        ChangeAnimation(ATTACK_ANIM);
    }

    // Call by Attack Animation
    public void Hit()
    {
        _playerHealth.TakeDamage(attackDamage);
    }

    // Call by the Health component 
    public void Die()
    {
        _state = AlienState.Dead;
        ChangeAnimation(DIE_ANIM);

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

    public void OnTakeDamage()
    {
        hitVFX.Play();
        if (_currentAnim != IDLE_ANIM && _currentAnim != MOVE_ANIM) return;

        ChangeAnimation(HURT_ANIM);
        Invoke(nameof(BackToPreviousAnim), _animator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void BackToPreviousAnim()
    {
        ChangeAnimation(_previousAnim);
    }
}