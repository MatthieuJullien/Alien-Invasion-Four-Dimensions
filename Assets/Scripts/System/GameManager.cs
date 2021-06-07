using UnityEngine;
using ECM.Controllers;
using System;

public enum PlayerViewPoint
{
    TopDown, SideView, FirstPerson, Isometric
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera isoCamera;

    [SerializeField] private Transform playerTransform;

    private Rigidbody playerRigidbody;
    private CharacterMovement playerMovement;
    private MultipleViewPlayerController playerController;
    private AudioListener fpsAudioListener;
    private AudioListener worldAudioListener;

    private int viewIndex = 0;
    public PlayerViewPoint ViewPoint { get; private set; }

    private void Start()
    {
        playerRigidbody = playerTransform.gameObject.GetComponent<Rigidbody>();
        playerMovement = playerTransform.gameObject.GetComponent<CharacterMovement>();
        playerController = playerTransform.gameObject.GetComponent<MultipleViewPlayerController>();

        worldCamera.orthographicSize = 8f;

        fpsAudioListener = fpsCamera.GetComponent<AudioListener>();
        worldAudioListener = worldCamera.GetComponent<AudioListener>();

        SelectTopDown();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //  ResetDefaultValues
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

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
    }

    private void UpdateAudioSetup() // AUDIO
    {
        if (ViewPoint != PlayerViewPoint.FirstPerson)
        {
            worldAudioListener.enabled = true;
            fpsAudioListener.enabled = false;
        }
        else
        {
            worldAudioListener.enabled = false;
            fpsAudioListener.enabled = true;
        }
    }

    private void UpdateControlSettings()
    {
        switch (ViewPoint)
        {
            case PlayerViewPoint.Isometric:
                playerController.baseJumpHeight = 1.8f;
                playerController.extraJumpPower = 25f;
                playerController.speed = 8f;
                playerController.runSpeedMultiplier = 1.6f;
                break;
            case PlayerViewPoint.TopDown:
                playerController.baseJumpHeight = 0f;
                playerController.extraJumpPower = 0f;
                playerController.speed = 8f;
                playerController.runSpeedMultiplier = 2;
                break;
            case PlayerViewPoint.SideView:
                playerController.baseJumpHeight = 2f;
                playerController.extraJumpPower = 25f;
                playerController.speed = 12f;
                playerController.runSpeedMultiplier = 1.5f;
                break;
            case PlayerViewPoint.FirstPerson:
                playerController.baseJumpHeight = 1.6f;
                playerController.extraJumpPower = 25f;
                playerController.speed = 12f;
                playerController.runSpeedMultiplier = 1.5f;
                break;
            default:
                break;
        }
    }


    public void SelectIso()
    {
        ViewPoint = PlayerViewPoint.Isometric;


        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = false;
        isoCamera.enabled = true;

        UpdateControlSettings();
        UpdateAudioSetup();
    }

    public void SelectTopDown()
    {
        ViewPoint = PlayerViewPoint.TopDown;

        // put arms horizontally
        Vector3 eulerAngles = fpsCamera.transform.rotation.eulerAngles;
        eulerAngles.x = 0f;
        fpsCamera.transform.rotation = Quaternion.Euler(eulerAngles);

        worldCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = true;
        isoCamera.enabled = false;

        UpdateControlSettings();
        UpdateAudioSetup();
    }

    public void SelectSideView()
    {
        ViewPoint = PlayerViewPoint.SideView;

        playerMovement.cachedRigidbody.MoveRotation(Quaternion.LookRotation(Vector3.right));
        playerRigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

        //worldCamera.orthographic = false;
        worldCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        fpsCamera.enabled = false;
        worldCamera.enabled = true;
        isoCamera.enabled = false;

        UpdateControlSettings();
        UpdateAudioSetup();
    }

    public void SelectFPS()
    {
        ViewPoint = PlayerViewPoint.FirstPerson;

        fpsCamera.enabled = true;
        worldCamera.enabled = false;
        isoCamera.enabled = false;

        UpdateControlSettings();
        UpdateAudioSetup();
    }
}
