using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float lifeDuration = 5f;

    protected float spawnTime;

    public abstract void Explode();

    protected void Start()
    {
        Physics.IgnoreLayerCollision(9, 9);
        Physics.IgnoreLayerCollision(9, 8);
        Physics.IgnoreLayerCollision(9, 7);
        Physics.IgnoreLayerCollision(9, 6);
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
        Explode();
    }
}
