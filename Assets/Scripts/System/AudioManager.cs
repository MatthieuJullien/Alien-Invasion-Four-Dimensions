using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private int worldListenerIndex = 0;
    [SerializeField] private int fpsListenerIndex = 1;
    [SerializeField] private float crossfadeSpeed = 0.5f;

    public FMOD.Studio.EventInstance PlayerWeaponsEvent { get; private set; }
    public FMOD.Studio.PARAMETER_ID WeaponSwitch { get; private set; }

    private FMOD.Studio.EventDescription playerWeaponsEventDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION playerParameterDescription;
    private int previousListenerIndex;
    private int currentListenerIndex;
    private bool crossFade = false;

    private void Start()
    {
        // Set listeners
        FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(fpsListenerIndex, 0f);
        FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(worldListenerIndex, 0f);

        PlayerWeaponsEvent = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Weapons");
        PlayerWeaponsEvent.getDescription(out playerWeaponsEventDescription);
        playerWeaponsEventDescription.getParameterDescriptionByName("weapons", out playerParameterDescription);
        WeaponSwitch = playerParameterDescription.id;
    }

    public void Play(FMOD.Studio.EventInstance pEvent)
    {
        pEvent.start();
    }

    public void SetLabeledParameter(FMOD.Studio.EventInstance pEvent, FMOD.Studio.PARAMETER_ID pID, int value)
    {
        pEvent.setParameterByID(pID, value, true);
    }

    private void Update()
    {
        if (crossFade)
        {
            CrossfadeBetweenListeners();
        }
    }

    private void CrossfadeBetweenListeners()
    {
        FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(currentListenerIndex, out float currentWeight);
        FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(previousListenerIndex, out float previousWeight);

        if (currentWeight < 1)
        {
            FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListenerIndex, currentWeight + crossfadeSpeed * Time.deltaTime);
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListenerIndex, 1f);
        }

        if (previousWeight > 0)
        {
            FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListenerIndex, previousWeight - crossfadeSpeed * Time.deltaTime);
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListenerIndex, 0);
        }

        if (currentWeight == 1 && previousWeight == 0)
        {
            crossFade = false;
        }
    }

    public void SetListenerCamera(bool shouldTargetWorldCam)
    {
        currentListenerIndex = shouldTargetWorldCam ? worldListenerIndex : fpsListenerIndex;
        previousListenerIndex = (currentListenerIndex + 1) % 2;
        crossFade = true;
    }
}