using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private float deathDuration = 0f;
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private Text healthBar;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        healthBar.text = "HP = " + _health.HealthPoints;
    }

    public void Die()
    {
        //_animator.SetTrigger(DieAnim);
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        GetComponent<MultipleViewPlayerController>().pause = true;
        Debug.Log("GAMEOVER !!!");
    }

    public void OnTakeDamage()
    {
        hitVFX.Play();
        healthBar.text = "HP = " + _health.HealthPoints;
    }
}
