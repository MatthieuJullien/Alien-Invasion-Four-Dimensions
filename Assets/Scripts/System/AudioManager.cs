using UnityEngine;

public class AudioManager : Singleton<AudioManager>

{
    private Camera currentCamera = null;
    private Camera previousCamera = null;
    private FMOD.ATTRIBUTES_3D currentAttributes = new FMOD.ATTRIBUTES_3D();
    private FMOD.ATTRIBUTES_3D previousAttributes = new FMOD.ATTRIBUTES_3D();

    public FMOD.Studio.EventInstance PlayerWeaponsEvent { get; private set; }
    public FMOD.Studio.PARAMETER_ID WeaponSwitch { get; private set; }
    private FMOD.Studio.EventDescription playerEventDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION playerParameterDescription;

    private int listenersNumber = 4;
    private int currentListener = 0;
    private int previousListener = 1;
    private bool crossFade = false;

    private void Start()
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

        PlayerWeaponsEvent = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Weapons");
        PlayerWeaponsEvent.getDescription(out playerEventDescription);
        playerEventDescription.getParameterDescriptionByName("weapons", out playerParameterDescription);
        WeaponSwitch = playerParameterDescription.id;
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
