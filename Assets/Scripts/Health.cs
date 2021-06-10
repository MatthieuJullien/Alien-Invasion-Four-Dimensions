using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealthPoints;
    private float currentHealthPoints;

    public bool IsDead { get => (currentHealthPoints <= 0); }

    private void Start()
    {
        currentHealthPoints = maxHealthPoints;
    }

    public void TakeDamage(float amount)
    {
        if (!IsDead)
        {
            currentHealthPoints -= amount;
        }
    }
}
