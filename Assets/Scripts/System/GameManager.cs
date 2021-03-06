using UnityEngine;

public enum PlayerViewPoint
{
    TopDown, SideView, FirstPerson, Isometric
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerViewPoint startingViewPoint;
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera isoCamera;
    [SerializeField] private MinimapCameraController minimap;

    [SerializeField] private Transform worldWeaponTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Movement Config ScriptableObjects")]
    [SerializeField] private MovementConfig topConfig;
    [SerializeField] private MovementConfig sideConfig;
    [SerializeField] private MovementConfig fpsMvtCfg;
    [SerializeField] private MovementConfig isoMvtCfg;

    private AudioManager audioMan;

    private Rigidbody playerRigidbody;
    private CharacterMovement playerMovement;
    private MultipleViewPlayerController playerController;

    private MovementConfig currentMvtCongif;
    //private bool isCurrentViewFPS;

    public bool IsCurrentViewFPS { get => (ViewPoint == PlayerViewPoint.FirstPerson); }

    public PlayerViewPoint ViewPoint { get; private set; }

    public override void Awake()
    {
        base.Awake();
        ViewPoint = startingViewPoint; // dangerous way to tell other class what is the starting viewpoint
    }

    private void Start()
    {
        audioMan = AudioManager.Instance;

        playerRigidbody = playerTransform.gameObject.GetComponent<Rigidbody>();
        playerMovement = playerTransform.gameObject.GetComponent<CharacterMovement>();
        playerController = playerTransform.gameObject.GetComponent<MultipleViewPlayerController>();

        worldCamera.orthographicSize = 8f;
        SelectViewPoint(startingViewPoint);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChangeToNextViewPoint();
        }
        //isCurrentViewFPS = (ViewPoint == PlayerViewPoint.FirstPerson);
    }


    private void ChangeToNextViewPoint()
    {
        PlayerViewPoint nextViewPoint = ViewPoint switch
        {
            PlayerViewPoint.TopDown => PlayerViewPoint.SideView,
            PlayerViewPoint.SideView => PlayerViewPoint.FirstPerson,
            PlayerViewPoint.FirstPerson => PlayerViewPoint.Isometric,
            PlayerViewPoint.Isometric => PlayerViewPoint.TopDown,
            _ => throw new System.NotImplementedException()
        };
        SelectViewPoint(nextViewPoint);
    }

    private void SelectViewPoint(PlayerViewPoint viewPoint)
    {
        switch (viewPoint)
        {
            case PlayerViewPoint.TopDown:
                SelectTopDown();
                break;
            case PlayerViewPoint.SideView:
                SelectSideView();
                break;
            case PlayerViewPoint.FirstPerson:
                SelectFPS();
                break;
            case PlayerViewPoint.Isometric:
                SelectIso();
                break;
            default:
                break;
        }
        UpdateControlSettings();
    }

    private void UpdateControlSettings()
    {
        switch (ViewPoint)
        {
            case PlayerViewPoint.Isometric:
                currentMvtCongif = isoMvtCfg;
                break;
            case PlayerViewPoint.TopDown:
                currentMvtCongif = topConfig;
                break;
            case PlayerViewPoint.SideView:
                currentMvtCongif = sideConfig;
                break;
            case PlayerViewPoint.FirstPerson:
                currentMvtCongif = fpsMvtCfg;
                break;
            default:
                break;
        }
        playerController.baseJumpHeight = currentMvtCongif.baseJumpHeight;
        playerController.extraJumpPower = currentMvtCongif.extraJumpPower;
        playerController.speed = currentMvtCongif.speed;
        playerController.runSpeedMultiplier = currentMvtCongif.runSpeedMultiplier;
        playerController.airControl = currentMvtCongif.airControl;
    }

    public void SelectIso()
    {
        //change listener
        audioMan.SetListenerCamera(isoCamera);

        ViewPoint = PlayerViewPoint.Isometric;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = false;
        isoCamera.enabled = true;
        minimap.ToggleViewpoint(IsCurrentViewFPS);
    }

    public void SelectTopDown()
    {
        audioMan.SetListenerCamera(worldCamera);

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
        minimap.ToggleViewpoint(IsCurrentViewFPS);
    }

    public void SelectSideView()
    {
        audioMan.SetListenerCamera(worldCamera);

        ViewPoint = PlayerViewPoint.SideView;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        playerMovement.cachedRigidbody.MoveRotation(Quaternion.LookRotation(Vector3.right));
        playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

        //worldCamera.orthographic = false;
        worldCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = true;
        isoCamera.enabled = false;
        minimap.ToggleViewpoint(IsCurrentViewFPS);
    }

    public void SelectFPS()
    {

        audioMan.SetListenerCamera(fpsCamera);

        ViewPoint = PlayerViewPoint.FirstPerson;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        fpsCamera.enabled = true;
        worldCamera.enabled = false;
        isoCamera.enabled = false;
        minimap.ToggleViewpoint(IsCurrentViewFPS);
    }
}
