using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private float maxSight = 12f;
    [SerializeField] private float deathDuration = 2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 10f;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("VFX")]
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;

    private Transform player;
    private bool isReadyToShoot = false;

    // ----------==[ Animsations ]==---------- //
    private Animator animator;
    private string _currentAnim;
    private string _previousAnim;
    private string futureAnim;

    const string ACTIVATION_ANIM = "turret_activation";
    const string SHOOT_ANIM = "turret_shoot";
    const string IDLE_ANIM = "desactivated";

    private void ChangeAnimation(string anim, float timescale = 1f)
    {
        if (_currentAnim == anim) return;

        animator.Play(anim, 0, timescale);
        CancelInvoke(nameof(BackToPreviousAnim));
        _previousAnim = _currentAnim;
        _currentAnim = anim;
    }

    private void ChangeToNextAnimation()
    {
        ChangeAnimation(futureAnim);
    }

    private void ProgramNextAnimation(string anim)
    {
        futureAnim = anim;
        Invoke(nameof(ChangeToNextAnimation), animator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void BackToPreviousAnim()
    {
        ChangeAnimation(_previousAnim);
    }
    // ----------==[[==========----------==========]]==---------- //

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerHandler>().transform;
    }

    private void Update()
    {
        float d = Vector3.Distance(transform.position, player.position);
        if (d < attackRange && isReadyToShoot)
        {
            ChangeAnimation(SHOOT_ANIM);
            print("play shoot");
        }
        else if (d < maxSight)
        {
            print("play activate");
            ChangeAnimation(ACTIVATION_ANIM);
        }
        else
        {
            ChangeAnimation(IDLE_ANIM);
            isReadyToShoot = false;
        }
    }

    private void AttackReady()
    {
        isReadyToShoot = true;
        print(name + " ready to shoot");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxSight);
    }


    // Call by shoot Animation
    public void Attack()
    {
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.Shoot();
    }

    public void OnTakeDamage()
    {
        hitVFX.Play();
    }

    // Call by the Health component 
    public void Die()
    {
        print(name + "died");
        ChangeAnimation(ACTIVATION_ANIM, -1f);
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject, deathDuration);
    }
}
