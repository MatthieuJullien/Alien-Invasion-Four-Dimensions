using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealthPoints;
    private float _currentHealthPoints;

    public bool IsDead { get; private set; }

    private void Start()
    {
        _currentHealthPoints = maxHealthPoints;
        IsDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        _currentHealthPoints -= amount;
        if (_currentHealthPoints <= 0)
        {
            IsDead = true;
            BroadcastMessage("Die");
        }
    }
}