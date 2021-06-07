using UnityEngine;
using UnityEngine.Assertions;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon[] worldWeaponList;
    [SerializeField] private Weapon[] fpsWeaponList;

    private int currentWeaponIndex = 0;
    private bool isCurrentViewFPS;
    private Weapon currentWeapon
    {
        get
        {
            if (isCurrentViewFPS)
                return fpsWeaponList[currentWeaponIndex];
            else
                return worldWeaponList[currentWeaponIndex];
        }
    }

    private void Start()
    {
        Assert.AreEqual(fpsWeaponList.Length, worldWeaponList.Length);

        for (int i = 0; i < fpsWeaponList.Length; i++)
        {
            fpsWeaponList[i].gameObject.SetActive(false);
            worldWeaponList[i].gameObject.SetActive(false);
        }
        isCurrentViewFPS = GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson;
        SwitchNextWeapon();
    }

    private void Update()
    {
        bool wasViewFPS = isCurrentViewFPS;
        isCurrentViewFPS = GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson;
        if (isCurrentViewFPS != wasViewFPS)
            UpdateActiveWeaponView();

        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchNextWeapon();
        }

        if (Input.GetMouseButton(0))
        {
            currentWeapon.Fire();
        }
    }

    private void SwitchNextWeapon()
    {
        currentWeapon.gameObject.SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + 1) % worldWeaponList.Length;
        UpdateActiveWeaponView();
    }

    private void UpdateActiveWeaponView()
    {
        fpsWeaponList[currentWeaponIndex].gameObject.SetActive(isCurrentViewFPS);
        worldWeaponList[currentWeaponIndex].gameObject.SetActive(!isCurrentViewFPS);
    }
}
