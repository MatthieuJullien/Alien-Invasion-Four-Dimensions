using UnityEngine;

public class AntiGravityGrenade : Projectile
{
    [SerializeField] private float throwForce = 650f;
    [SerializeField] private float explosionRadius = 20f;
    [SerializeField] private float pullPower = 500;
    [SerializeField] private GameObject explosionVFX;

    public override void Shoot()
    {
        base.Shoot();
        GetComponent<Rigidbody>().AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    public override void Explode()
    {
        Instantiate(explosionVFX, transform.position, transform.rotation);

        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 pushVector = explosionPosition - rb.position;
                rb.AddForce(pushVector.normalized * pullPower);
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // nothing to do
    }

    protected override void Update()
    {
        if (Time.time >= spawnTime + lifeDuration)
        {
            Explode();
            Destroy(gameObject);
        }
    }
}
