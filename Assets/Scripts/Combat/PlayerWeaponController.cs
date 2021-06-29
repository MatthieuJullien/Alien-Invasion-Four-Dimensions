using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private float switchWeaponCooldown = 0.2f;

    private float weaponSwitchTimer = Mathf.Infinity;
    private int currentWeaponIndex;
    private bool isCurrentViewFPS;

    private void OnEnable()
    {
        isCurrentViewFPS = GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson;
        weaponSwitchTimer = Mathf.Infinity;
        EquipWeapon(0);
    }

    private void Update()
    {
        bool wasViewFPS = isCurrentViewFPS;
        isCurrentViewFPS = GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson;
        if (isCurrentViewFPS != wasViewFPS)
        {
            weapons[currentWeaponIndex].UpdateModelView(isCurrentViewFPS);
        }

        weaponSwitchTimer += Time.deltaTime;
        weapons[currentWeaponIndex].ReloadTick();
        HandleWeaponInputs();
    }

    private void HandleWeaponInputs()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            SwitchNextWeapon(Input.mouseScrollDelta.y > 0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.R))
        {
            EquipWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.T))
        {
            EquipWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.F))
        {
            EquipWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.G))
        {
            EquipWeapon(3);
        }

        if (Input.GetMouseButton(0))
        {
            weapons[currentWeaponIndex].Fire();
        }
    }

    private void SwitchNextWeapon(bool ascendingOrder)
    {
        int weaponIndexDelta = ascendingOrder ? 1 : -1;
        int nextWeaponIndex = (currentWeaponIndex + weaponIndexDelta + weapons.Length) % weapons.Length;
        EquipWeapon(nextWeaponIndex);
    }

    public void EquipWeapon(int weaponToEquipIndex)
    {
        if (weaponSwitchTimer < switchWeaponCooldown)
        {
            return;
        }

        weaponSwitchTimer = 0f;
        weapons[currentWeaponIndex].Equip(false);
        currentWeaponIndex = weaponToEquipIndex;
        weapons[weaponToEquipIndex].Equip(true);
    }
}
