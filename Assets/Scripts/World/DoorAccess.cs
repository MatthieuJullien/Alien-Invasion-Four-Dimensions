using UnityEngine;

public class DoorAccess : MonoBehaviour
{
    [SerializeField] private Light[] doorLights;
    [SerializeField] private ObjectiveEnum requiredObjective;

    [SerializeField] private float openedLightIntensity;
    [SerializeField] private float closedLightIntensity;
    [SerializeField] private Color deniedColor;
    [SerializeField] private Color openedColor;
    [SerializeField] private Color closedColor;

    private Animator _animator;
    private bool _hasAccess = false;

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
    }

    public void TryOpenDoor()
    {
        Color color;
        if (_hasAccess)
        {
            color = openedColor;
            if (_animator != null)
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
        if (_animator != null)
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
        if (objectiveLabel != requiredObjective) return;
        _hasAccess = true;
    }
}
