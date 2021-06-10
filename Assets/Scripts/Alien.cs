using UnityEngine;

public class Alien : MonoBehaviour
{
    [SerializeField] private GameObject deathVFX;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.IsDead)
        {
            Die();
            return;
        }
    }

    private void Die()
    {
        Instantiate(deathVFX, transform);
        Destroy(gameObject);
    }
}
