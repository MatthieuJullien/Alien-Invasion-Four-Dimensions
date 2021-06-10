using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] private float moveForce = 200f;

    public override void Shoot()
    {
        base.Shoot();
        GetComponent<Rigidbody>().velocity = transform.forward * moveForce;
    }

    public override void Explode()
    {
        Destroy(gameObject);
    }
}
