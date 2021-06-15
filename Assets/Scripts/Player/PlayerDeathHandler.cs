using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private float deathDuration = 0f;
    [SerializeField] private GameObject deathVFX;

    public void Die()
    {
        //_animator.SetTrigger(DieAnim);
        Instantiate(deathVFX, transform.position, Quaternion.identity);
        GetComponent<MultipleViewPlayerController>().pause = true;
    }
}
