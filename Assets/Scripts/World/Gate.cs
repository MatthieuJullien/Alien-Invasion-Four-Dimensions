using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;
    [SerializeField] private AudioClip movingSound;

    private TwoPositionBlock door;
    private TriggerZone triggerZone;
    private AudioSource audioSource;

    private void Awake()
    {
        door = GetComponentInChildren<TwoPositionBlock>();
        triggerZone = GetComponentInChildren<TriggerZone>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        if (!isOpened)
        {
            audioSource.PlayOneShot(movingSound);
            isOpened = true;
            door.MoveToTarget();
            triggerZone.enabled = false;
        }
    }
}
