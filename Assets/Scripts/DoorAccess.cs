using UnityEngine;

public class DoorAccess : MonoBehaviour
{
    [SerializeField] private Light doorlight;
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
    }

    private void Start()
    {
        ObjectiveManager.Instance.CompletedEvent.AddListener(OnAccessGained);
        doorlight.color = closedColor;
        doorlight.intensity = closedLightIntensity;
    }

    public void TryOpenDoor()
    {
        doorlight.intensity = openedLightIntensity;
        if (_hasAccess)
        {
            doorlight.color = openedColor;
            _animator.SetTrigger("character_nearby");
        }
        else
        {
            doorlight.color = deniedColor;
        }
        Invoke(nameof(ResetLight), 2.5f);
    }

    public void TryCloseDoor()
    {
        _animator.ResetTrigger("character_nearby");
    }

    private void ResetLight()
    {
        doorlight.color = closedColor;
        doorlight.intensity = closedLightIntensity;
    }

    public void OnAccessGained(ObjectiveEnum objectiveLabel)
    {
        if (objectiveLabel != ObjectiveEnum.GetAccessCodes) return;
        _hasAccess = true;
    }
}
