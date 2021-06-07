using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f; // TODO not use by physic moving projectiles
    [SerializeField] protected float lifeDuration = 5f;
    [SerializeField] private bool isKinematic;

    public bool IsKinematic { get => isKinematic; }

    protected float spawnTime;

    protected void Start()
    {
        spawnTime = Time.time;
    }

    protected virtual void Update()
    {
        if (!IsKinematic)
            transform.Translate(speed * Time.deltaTime * Vector3.forward);

        if (Time.time >= spawnTime + lifeDuration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
