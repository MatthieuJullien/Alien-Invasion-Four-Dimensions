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
    [SerializeField] private Fader fader;

    [SerializeField] private Transform worldWeaponTransform;
    [SerializeField] private Transform playerTransform;

    [Header("Movement Config ScriptableObjects")]
    [SerializeField] private MovementConfig topConfig;
    [SerializeField] private MovementConfig sideConfig;
    [SerializeField] private MovementConfig fpsMvtCfg;
    [SerializeField] private MovementConfig isoMvtCfg;

    [Header("Debug")]
    [SerializeField] private GameObject alienQueenPrefab;
    [SerializeField] private GameObject cheatCanvas;

    public void SpawnQueen()
    {
        var pos = playerTransform.position;
        pos.y += 2f;
        Instantiate(alienQueenPrefab, pos, Quaternion.identity);
    }

    private AudioManager audioMan;

    private Rigidbody playerRigidbody;
    private CharacterMovement playerMovement;
    private MultipleViewPlayerController playerController;

    private MovementConfig currentMvtCongif;
    //private bool isCurrentViewFPS;

    public bool IsCurrentViewFPS { get => (ViewPoint == PlayerViewPoint.FirstPerson); }
    public PlayerViewPoint ViewPoint { get; private set; }
    public Camera CurrentCamera { get; private set; }



    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        audioMan = AudioManager.Instance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
            bool shouldShowCheats = !cheatCanvas.activeSelf;
            cheatCanvas.SetActive(shouldShowCheats);
            Cursor.lockState = shouldShowCheats ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldShowCheats;
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
    }

    private void SetControlSettings()
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
        if (ViewPoint == PlayerViewPoint.Isometric) return;

        audioMan.SetListenerCamera(isoCamera);
        fader.CameraSwitchEvent.RemoveAllListeners();
        fader.CameraSwitchEvent.AddListener(Iso);
        fader.CameraTransition();
    }

    public void SelectTopDown()
    {
        if (ViewPoint == PlayerViewPoint.TopDown) return;

        audioMan.SetListenerCamera(worldCamera);
        fader.CameraSwitchEvent.RemoveAllListeners();
        fader.CameraSwitchEvent.AddListener(TopDown);
        fader.CameraTransition();
    }

    public void SelectSideView()
    {
        if (ViewPoint == PlayerViewPoint.SideView) return;

        audioMan.SetListenerCamera(worldCamera);
        fader.CameraSwitchEvent.RemoveAllListeners();
        fader.CameraSwitchEvent.AddListener(SideView);
        fader.CameraTransition();
    }

    public void SelectFPS()
    {
        if (ViewPoint == PlayerViewPoint.FirstPerson) return;

        audioMan.SetListenerCamera(fpsCamera);
        fader.CameraSwitchEvent.RemoveAllListeners();
        fader.CameraSwitchEvent.AddListener(Fps);
        fader.CameraTransition();
    }

    private void Iso()
    {
        ViewPoint = PlayerViewPoint.Isometric;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        PutArmsHorizontally();

        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        SetCamera(isoCamera);
        minimap.ToggleViewpoint(IsCurrentViewFPS);
        SetControlSettings();
    }

    private void TopDown()
    {
        ViewPoint = PlayerViewPoint.TopDown;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        PutArmsHorizontally();

        SetCamera(worldCamera);
        minimap.ToggleViewpoint(IsCurrentViewFPS);
        SetControlSettings();
    }

    private void SideView()
    {
        ViewPoint = PlayerViewPoint.SideView;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        playerMovement.cachedRigidbody.MoveRotation(Quaternion.LookRotation(Vector3.right));
        playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

        //worldCamera.orthographic = false;
        worldCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        SetCamera(worldCamera);
        minimap.ToggleViewpoint(IsCurrentViewFPS);
        SetControlSettings();
    }

    private void Fps()
    {
        ViewPoint = PlayerViewPoint.FirstPerson;

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        SetCamera(fpsCamera);
        minimap.ToggleViewpoint(IsCurrentViewFPS);
        SetControlSettings();
    }

    public void SetCamera(Camera cam)
    {
        fpsCamera.enabled = fpsCamera == cam;
        worldCamera.enabled = worldCamera == cam;
        isoCamera.enabled = isoCamera == cam;
        CurrentCamera = cam;
    }

    private void PutArmsHorizontally()
    {
        Vector3 eulerAngles = fpsCamera.transform.rotation.eulerAngles;
        eulerAngles.x = 0f;
        fpsCamera.transform.rotation = Quaternion.Euler(eulerAngles);
        worldWeaponTransform.rotation = Quaternion.Euler(eulerAngles);
        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
