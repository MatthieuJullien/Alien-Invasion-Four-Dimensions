using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform[] projectileSpawnPoints;
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] private float throwForce;
    [SerializeField] private float reloadDuration;

    private Animation animationReader;
    private bool isLoaded = true;
    private float lastFireTime;

    private void Awake()
    {
        animationReader = GetComponent<Animation>();
    }

    private void Start()
    {
        animationReader.clip.legacy = true;
        if (animationReader.clip.length > reloadDuration)
        {
            Debug.Log($"The reload duration of {gameObject.name} must be longer than the shooting animation (anim = {animationReader.clip.length}).");
        }
    }

    public void Fire()
    {
        if (!isLoaded)
            return;

        foreach (Transform spawn in projectileSpawnPoints)
        {
            Projectile projectile = Instantiate<Projectile>(projectilePrefab, spawn.position, spawn.rotation);
            if (projectilePrefab.IsKinematic)
            {
                projectile.GetComponent<Rigidbody>().AddForce(spawn.forward * throwForce);
            }
        }
        animationReader.Play();
        lastFireTime = Time.time;
        isLoaded = false;
    }

    private void Update()
    {
        if (Time.time >= lastFireTime + reloadDuration)
        {
            isLoaded = true;
        }
    }
}