public class AudioManager : Singleton<AudioManager>

{

    //private static AudioManager instanceAudioMan;

    public FMOD.Studio.EventInstance PlayerWeapons { get; private set; }
    public FMOD.Studio.PARAMETER_ID WeaponSwitch { get; private set; }

    private FMOD.Studio.EventDescription playerEventDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION playerParameterDescription;


    private AudioManager()
    {
        PlayerWeapons = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Weapons");
        PlayerWeapons.getDescription(out playerEventDescription);
        playerEventDescription.getParameterDescriptionByName("weapons", out playerParameterDescription);
        WeaponSwitch = playerParameterDescription.id;
    }

    /*
    public static AudioManager GetInstance()
    {
        if (instanceAudioMan == null)
        {
            instanceAudioMan = new AudioManager();
        }
        return instanceAudioMan;
    }*/


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

}
