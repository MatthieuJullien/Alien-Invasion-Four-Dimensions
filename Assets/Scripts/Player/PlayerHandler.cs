using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private Text healthBar;
    [SerializeField] private GameObject fpsWorkTools;
    [SerializeField] private GameObject worldMechanicModels;
    [SerializeField] private GameObject worldNoUniformModels;
    [SerializeField] private Crosshair crosshair;

    private Health _health;
    private PlayerWeaponController _playerWeaponController;

    public bool HasMechanicUniform { get; private set; }

    private void Awake()
    {
        _health = GetComponent<Health>();
        _playerWeaponController = GetComponentInChildren<PlayerWeaponController>();
    }

    private void Start()
    {
        HasMechanicUniform = true;
        _playerWeaponController.enabled = false;
        worldNoUniformModels.SetActive(false);
        healthBar.text = "HP = " + _health.HealthPoints;
    }

    private void Update()
    {
        if (HasMechanicUniform)
        {
            bool fpsView = GameManager.Instance.IsCurrentViewFPS;
            fpsWorkTools.SetActive(fpsView);
            worldMechanicModels.SetActive(!fpsView);
        }

        // player is in the wrong place
        if (transform.position.y < -20f)
        {
            _health.TakeDamage(100f);
        }
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

    public void HackAccessCodes()
    {
        print("You have hacked the system and stolen then pilot codes!");
        Soldier.CodeRed = true;
    }

    // replace with pickups + weapon inventory
    public void TakeWeapons()
    {
        HasMechanicUniform = false;

        fpsWorkTools.SetActive(false);
        worldMechanicModels.SetActive(false);
        worldNoUniformModels.SetActive(true);
        _playerWeaponController.enabled = true;
        _playerWeaponController.EquipWeapon(0);

        crosshair.HasWeapons = true;
        crosshair.UpdateCrosshairVisibility();
    }
}
