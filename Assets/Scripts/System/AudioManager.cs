using UnityEngine;

public class AudioManager : Singleton<AudioManager>

{
    private Camera currentCamera = null;
    private Camera previousCamera = null;
    FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();

    public FMOD.Studio.EventInstance PlayerWeapons { get; private set; }
    public FMOD.Studio.PARAMETER_ID WeaponSwitch { get; private set; }
    private FMOD.Studio.EventDescription playerEventDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION playerParameterDescription;

    private int currentListener = 0;
    private int previousListener = 1;
    private bool crossFade = false;

    private AudioManager()
    {
    }

    private void Start()
    {
        //Set listeners
        FMODUnity.RuntimeManager.StudioSystem.setNumListeners(2);
        FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListener, 1);
        FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListener, 0);

        PlayerWeapons = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Weapons");
        PlayerWeapons.getDescription(out playerEventDescription);
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
        attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.position);
        attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.forward);
        attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(currentCamera.transform.up);
        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(currentListener, attributes);

        if (crossFade)
        {
            FMODUnity.RuntimeManager.StudioSystem.getNumListeners(out int num);

            FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(currentListener, out float currentWeight);
            FMODUnity.RuntimeManager.StudioSystem.getListenerWeight(previousListener, out float previousWeight);

            if (currentWeight < 1)
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListener, currentWeight + 60 * Time.deltaTime);
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(currentListener, 1);
            }

            if (previousWeight > 0)
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListener, previousWeight - 60 * Time.deltaTime);
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setListenerWeight(previousListener, 0);
            }

            attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.position);
            attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.forward);
            attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(previousCamera.transform.up);
            FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(previousListener, attributes);

            if (currentWeight == 1 && previousWeight == 0)
            {
                previousListener = currentListener;
                currentListener = 0;
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
            currentListener = 1;
            crossFade = true;
        }
        currentCamera = cam;
    }

}
