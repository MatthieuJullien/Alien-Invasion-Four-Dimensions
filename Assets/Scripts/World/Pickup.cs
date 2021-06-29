using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    [SerializeField] private UnityEvent pickupAction;
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private Transform pickupModelTransform;

    [SerializeField] private float height = 0.4f;

    [Tooltip("If respawn interval less or equals to zero then the pickup does not respawn")]
    [SerializeField] private float respawnInterval = 5f;

    private bool canBePickup = true;
    private Vector3 defaultPosition;

    private bool willBeDestroyAfterEffects = false;

    private void Update()
    {
        if (willBeDestroyAfterEffects && !pickupEffect.isPlaying)
            Destroy(this);

        // rotate pickup
        float angle = 45f;
        pickupModelTransform.Rotate(Vector3.up, angle * Time.deltaTime);
        pickupModelTransform.position = transform.position + Vector3.up * Mathf.PingPong(Time.time * 0.1f, height);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBePickup)
            return;

        PlayerHandler player = other.gameObject.GetComponent<PlayerHandler>();
        if (player != null)
        {
            if (pickupAction.GetPersistentEventCount() > 0)
                pickupAction.Invoke();

            pickupEffect.Play();
            pickupModelTransform.gameObject.SetActive(false);

            canBePickup = false;
            if (respawnInterval <= 0f)
            {
                willBeDestroyAfterEffects = true;
                //Destroy(gameObject);
            }
            else
            {
                Invoke(nameof(Respawn), respawnInterval);
            }
        }
    }

    private void Respawn()
    {
        pickupModelTransform.gameObject.SetActive(true);
        canBePickup = true;
    }

}
