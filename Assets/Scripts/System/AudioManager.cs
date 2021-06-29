using UnityEngine;

public class AudioManager : Singleton<AudioManager>

{
    private Camera currentCamera = null;
    private Camera previousCamera = null;
    private FMOD.ATTRIBUTES_3D currentAttributes = new FMOD.ATTRIBUTES_3D();
    private FMOD.ATTRIBUTES_3D previousAttributes = new FMOD.ATTRIBUTES_3D();


    public FMOD.Studio.EventInstance guiObjectives { get; private set; }
    public FMOD.Studio.EventInstance backgroundAmbient { get; private set; }
    public FMOD.Studio.EventInstance playerWeaponsEvent { get; private set; }
    public FMOD.Studio.PARAMETER_ID weaponSwitch { get; private set; }
    public FMOD.Studio.EventInstance doorEvent { get; private set; }
    public FMOD.Studio.PARAMETER_ID doorSwitch { get; private set; }

    private int listenersNumber = 4;
    private int currentListener = 0;
    private int previousListener = 1;
    private bool crossFade = false;

    private void Awake()
    {
        //Set listeners
        FMODUnity.RuntimeManager.StudioSystem.setNumListeners(listenersNumber);
        for (int i = 0; i < listenersNumber; i++)
        {
            if (i == 0)
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(i, 1);
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(i, 0);
            }
        }

        guiObjectives = FMODUnity.RuntimeManager.CreateInstance("event:/UI/ObjectiveComplete");
        backgroundAmbient = FMODUnity.RuntimeManager.CreateInstance("event:/Ambient/Background");
        playerWeaponsEvent = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Weapons");
        playerWeaponsEvent.getDescription(out FMOD.Studio.EventDescription playerEventDescription);
        playerEventDescription.getParameterDescriptionByName("weapons", out FMOD.Studio.PARAMETER_DESCRIPTION playerParameterDescription);
        weaponSwitch = playerParameterDescription.id;

        doorEvent = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Objects/Door");
        doorEvent.getDescription(out FMOD.Studio.EventDescription doorEventDescription);
        doorEventDescription.getParameterDescriptionByName("isOpen", out FMOD.Studio.PARAMETER_DESCRIPTION doorParameterDescription);
        doorSwitch = doorParameterDescription.id;

    }

    private void Start()
    {
        backgroundAmbient.start();
    }

    public void Play(FMOD.Studio.EventInstance pEvent)
    {
        pEvent.start();
    }


    public void SetLabeledParameter(FMOD.Studio.EventInstance pEvent, FMOD.Studio.PARAMETER_ID pID, int value)
    {
        {
            pEvent.setParameterByID(pID, value, true);
        }
    }

    private void Update()
    {
        currentAttributes.position = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.position);
        currentAttributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.forward);
        currentAttributes.up = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.up);
        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(currentListener, currentAttributes);

        if (crossFade)
        {
            FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(currentListener, out float currentWeight);
            FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(previousListener, out float previousWeight);

            if (currentWeight < 1)
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListener, currentWeight + Time.deltaTime);
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListener, 1);
            }

            if (previousWeight > 0)
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListener, previousWeight - Time.deltaTime);
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListener, 0);
            }

            FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(previousListener, previousAttributes);

            if (currentWeight == 1 && previousWeight == 0)
            {
                crossFade = false;
            }
        }
    }


    public void SetListenerCamera(Camera cam)
    {
        if (currentCamera != null)
        {
            previousCamera = currentCamera;
            previousListener = currentListener;
            currentListener++;
            if (currentListener > listenersNumber - 1)
            {
                currentListener = 0;
            }
            previousAttributes.position = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.position);
            previousAttributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.forward);
            previousAttributes.up = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.up);

            crossFade = true;
        }
        currentCamera = cam;
    }

}
