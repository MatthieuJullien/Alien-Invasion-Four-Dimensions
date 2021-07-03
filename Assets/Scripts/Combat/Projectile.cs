using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float lifeDuration = 5f;
    [SerializeField] protected float damage = 10;
    [SerializeField] private GameObject hitVFX;

    private Vector3 from;

    protected float spawnTime;

    public abstract void Explode();

    protected void Start()
    {
        Physics.IgnoreLayerCollision(9, 9); // ProjectilePlayer
        Physics.IgnoreLayerCollision(9, 8); // Player
        Physics.IgnoreLayerCollision(9, 7); // FirstPersonView
        Physics.IgnoreLayerCollision(9, 6); // WorldView
        from = transform.position;
    }

    public virtual void Shoot()
    {
        spawnTime = Time.time;
    }


    protected virtual void Update()
    {
        if (Time.time >= spawnTime + lifeDuration)
        {
            Explode();
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Quaternion hitOrientation = Quaternion.LookRotation(collision.GetContact(0).normal);
        Instantiate(hitVFX, transform.position, hitOrientation);
        InflictDamage(collision);
        Explode();
    }

    protected void InflictDamage(Collision collision)
    {
        Health otherHealth = collision.gameObject.GetComponent<Health>();
        if (otherHealth != null)
        {
            otherHealth.TakeDamage(damage);
        }
    }
}
