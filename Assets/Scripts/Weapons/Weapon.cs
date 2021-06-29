using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private float reloadDuration = 1f;
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected Transform[] fpsProjectileSpawnPoints;
    [SerializeField] protected Transform[] worldProjectileSpawnPoints;
    [SerializeField] private GameObject fpsModel;
    [SerializeField] private GameObject worldModel;
    [SerializeField] private int audioWeaponSwitchValue = 0;


    private float reloadTimer = Mathf.NegativeInfinity;
    private Animation animationReader;
    private Animation fpsAnim;
    private Animation worldAnim;

    // Audio
    private GameObject player;
    private Rigidbody playerRigidbody;
    private static AudioManager audioMan;
    private FMOD.Studio.EventInstance weapons;
    private FMOD.Studio.PARAMETER_ID wSwitch;

    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        animationReader = GetComponent<Animation>();
        fpsAnim = fpsModel.GetComponent<Animation>();
        worldAnim = worldModel.GetComponent<Animation>();
    }

    private void Start()
    {
        IsLoaded = true;
        fpsModel.SetActive(false);
        worldModel.SetActive(false);
        animationReader.clip.legacy = true;
        if (animationReader.clip.length > reloadDuration)
        {
            Debug.Log($"The reload duration of {gameObject.name} must be longer than the shooting animation (anim = {animationReader.clip.length}).");
        }

        audioMan = AudioManager.Instance;
        weapons = audioMan.playerWeaponsEvent;
        wSwitch = audioMan.weaponSwitch;
    }

    private void Update()
    {
        if (reloadTimer >= reloadDuration)
        {
            IsLoaded = true;
        }
    }

    public void ReloadTick()
    {
        reloadTimer += Time.deltaTime;
    }

    public void UpdateModelView(bool isCurrentViewFPS)
    {
        fpsModel.SetActive(isCurrentViewFPS);
        worldModel.SetActive(!isCurrentViewFPS);
    }

    public void Equip(bool shouldEquip)
    {
        if (!shouldEquip)
        {
            fpsModel.SetActive(false);
            worldModel.SetActive(false);
        }
        else
        {
            UpdateModelView(GameManager.Instance.IsCurrentViewFPS);
        }
    }

    public void Fire()
    {
        if (!IsLoaded)
            return;

        //update 3D position
        weapons.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        //setting switch to current weapon
        audioMan.SetLabeledParameter(weapons, wSwitch, audioWeaponSwitchValue);
        //playing sound
        audioMan.Play(weapons);

        Transform[] spawnPoints;
        if (GameManager.Instance.IsCurrentViewFPS)
        {
            fpsAnim.Play();
            spawnPoints = fpsProjectileSpawnPoints;
        }
        else
        {
            worldAnim.Play();
            spawnPoints = worldProjectileSpawnPoints;
        }

        foreach (Transform spawn in spawnPoints)
        {
            Projectile projectile = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
            projectile.Shoot();
        }
        reloadTimer = 0f;
        IsLoaded = false;
    }
}