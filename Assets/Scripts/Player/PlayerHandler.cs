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

    private Health _health;
    private PlayerWeaponController _playerWeaponController;
    private bool _hasMechanicUniform;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _playerWeaponController = GetComponentInChildren<PlayerWeaponController>();
    }

    private void Start()
    {
        _hasMechanicUniform = true;
        _playerWeaponController.enabled = false;
        worldNoUniformModels.SetActive(false);
        healthBar.text = "HP = " + _health.HealthPoints;
    }

    private void Update()
    {
        if (_hasMechanicUniform)
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
    }

    // replace with pickups + weapon inventory
    public void TakeWeapons()
    {
        _hasMechanicUniform = false;
        fpsWorkTools.SetActive(false);
        worldMechanicModels.SetActive(false);
        worldNoUniformModels.SetActive(true);
        _playerWeaponController.enabled = true;
        _playerWeaponController.EquipWeapon(0);
    }
}
