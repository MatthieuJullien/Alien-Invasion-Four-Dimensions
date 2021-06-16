using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealthPoints;
    [SerializeField] private UnityEvent TakeDamageEvent;


    private float _currentHealthPoints;

    public bool IsDead { get; private set; }
    public int HealthPoints { get => Mathf.CeilToInt(_currentHealthPoints); }


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
        TakeDamageEvent.Invoke();
    }
}