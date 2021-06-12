using UnityEngine;

public enum PlayerViewPoint
{
    TopDown, SideView, FirstPerson, Isometric
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera isoCamera;
    [SerializeField] private Transform worldWeaponTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Movement Config ScriptableObjects")]
    [SerializeField] private MovementConfig topConfig;
    [SerializeField] private MovementConfig sideConfig;
    [SerializeField] private MovementConfig fpsMvtCfg;
    [SerializeField] private MovementConfig isoMvtCfg;


    private Rigidbody playerRigidbody;
    private CharacterMovement playerMovement;
    private MultipleViewPlayerController playerController;

    private int viewIndex = 0;
    private bool isCurrentViewFPS;
    public bool IsCurrentViewFPS { get => isCurrentViewFPS; }

    public PlayerViewPoint ViewPoint { get; private set; }

    private void Start()
    {
        playerRigidbody = playerTransform.gameObject.GetComponent<Rigidbody>();
        playerMovement = playerTransform.gameObject.GetComponent<CharacterMovement>();
        playerController = playerTransform.gameObject.GetComponent<MultipleViewPlayerController>();

        worldCamera.orthographicSize = 8f;

        SelectTopDown();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            viewIndex = (viewIndex + 1) % 4;
            switch (viewIndex)
            {
                case 0:
                    SelectTopDown();
                    break;
                case 1:
                    SelectSideView();
                    break;
                case 2:
                    SelectFPS();
                    break;
                case 3:
                    SelectIso();
                    break;
                default:
                    break;
            }
        }
        isCurrentViewFPS = GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson;
    }

    private void UpdateControlSettings()
    {
        switch (ViewPoint)
        {
            case PlayerViewPoint.Isometric:
                playerController.baseJumpHeight = isoMvtCfg.baseJumpHeight;
                playerController.extraJumpPower = isoMvtCfg.extraJumpPower;
                playerController.speed = isoMvtCfg.speed;
                playerController.runSpeedMultiplier = isoMvtCfg.runSpeedMultiplier;
                playerController.airControl = isoMvtCfg.airControl;
                break;
            case PlayerViewPoint.TopDown:
                playerController.baseJumpHeight = topConfig.baseJumpHeight;
                playerController.extraJumpPower = topConfig.extraJumpPower;
                playerController.speed = topConfig.speed;
                playerController.runSpeedMultiplier = topConfig.runSpeedMultiplier;
                playerController.airControl = topConfig.airControl;
                break;
            case PlayerViewPoint.SideView:
                playerController.baseJumpHeight = sideConfig.baseJumpHeight;
                playerController.extraJumpPower = sideConfig.extraJumpPower;
                playerController.speed = sideConfig.speed;
                playerController.runSpeedMultiplier = sideConfig.runSpeedMultiplier;
                playerController.airControl = sideConfig.airControl;
                break;
            case PlayerViewPoint.FirstPerson:
                playerController.baseJumpHeight = fpsMvtCfg.baseJumpHeight;
                playerController.extraJumpPower = fpsMvtCfg.extraJumpPower;
                playerController.speed = fpsMvtCfg.speed;
                playerController.runSpeedMultiplier = fpsMvtCfg.runSpeedMultiplier;
                playerController.airControl = fpsMvtCfg.airControl;
                break;
            default:
                break;
        }
    }


    public void SelectIso()
    {
        ViewPoint = PlayerViewPoint.Isometric;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = false;
        isoCamera.enabled = true;

        UpdateControlSettings();
    }

    public void SelectTopDown()
    {
        ViewPoint = PlayerViewPoint.TopDown;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        // put arms horizontally
        Vector3 eulerAngles = fpsCamera.transform.rotation.eulerAngles;
        eulerAngles.x = 0f;
        fpsCamera.transform.rotation = Quaternion.Euler(eulerAngles);
        worldWeaponTransform.rotation = Quaternion.Euler(eulerAngles);
        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = true;
        isoCamera.enabled = false;

        UpdateControlSettings();
    }

    public void SelectSideView()
    {
        ViewPoint = PlayerViewPoint.SideView;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        playerMovement.cachedRigidbody.MoveRotation(Quaternion.LookRotation(Vector3.right));
        playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

        //worldCamera.orthographic = false;
        worldCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = true;
        isoCamera.enabled = false;

        UpdateControlSettings();
    }

    public void SelectFPS()
    {
        ViewPoint = PlayerViewPoint.FirstPerson;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        fpsCamera.enabled = true;
        worldCamera.enabled = false;
        isoCamera.enabled = false;

        UpdateControlSettings();
    }
}
