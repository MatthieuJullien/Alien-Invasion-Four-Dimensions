using UnityEngine;

public class DoorAccess : MonoBehaviour
{
    [SerializeField] private Light[] doorLights;
    [SerializeField] private float openedLightIntensity;
    [SerializeField] private float closedLightIntensity;
    [SerializeField] private Color deniedColor;
    [SerializeField] private Color openedColor;
    [SerializeField] private Color closedColor;

    private Animator _animator;
    private bool _hasAccess = false;

    private static AudioManager audioMan;
    private FMOD.Studio.EventInstance doorEvent;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        doorLights = GetComponentsInChildren<Light>();
    }

    private void Start()
    {
        ObjectiveManager.Instance.CompletedEvent.AddListener(OnAccessGained);
        foreach (var l in doorLights)
        {
            l.color = closedColor;
            l.intensity = closedLightIntensity;
        }

        audioMan = AudioManager.Instance;
        doorEvent = audioMan.doorEvent;
        Debug.Log("doorEvent is valid: " +doorEvent.isValid());
    }

    public void TryOpenDoor()
    {
        Debug.Log("doorEvent is valid: " +doorEvent.isValid());
        Debug.Log(doorEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject)));
        audioMan.Play(doorEvent);

        Color color;
        if (_hasAccess)
        {
            color = openedColor;
            _animator.SetTrigger("character_nearby");
        }
        else
        {
            color = deniedColor;
        }

        foreach (var l in doorLights)
        {
            l.color = color;
            l.intensity = openedLightIntensity;
        }
        Invoke(nameof(ResetLight), 2.5f);
    }

    public void TryCloseDoor()
    {
        _animator.ResetTrigger("character_nearby");
    }

    private void ResetLight()
    {
        foreach (var l in doorLights)
        {
            l.color = closedColor;
            l.intensity = closedLightIntensity;
        }
    }

    public void OnAccessGained(ObjectiveEnum objectiveLabel)
    {
        if (objectiveLabel != ObjectiveEnum.GetAccessCodes) return;
        _hasAccess = true;
    }
}
