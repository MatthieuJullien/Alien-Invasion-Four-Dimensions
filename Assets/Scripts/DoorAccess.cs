using UnityEngine;

public class DoorAccess : MonoBehaviour
{
    [SerializeField] private Light spotlight;
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
        spotlight.color = closedColor;
        spotlight.intensity = 0.5f;
    }

    public void TryOpenDoor()
    {
        spotlight.intensity = openedLightIntensity;
        if (_hasAccess)
        {
            spotlight.color = openedColor;
            _animator.SetTrigger("character_nearby");
        }
        else
        {
            spotlight.color = deniedColor;
        }
    }

    public void TryCloseDoor()
    {
        _animator.ResetTrigger("character_nearby");
        spotlight.color = closedColor;
        spotlight.intensity = closedLightIntensity;
    }

    public void OnAccessGained(ObjectiveEnum objectiveLabel)
    {
        _hasAccess = true;
        if (objectiveLabel != ObjectiveEnum.GetAccessCodes) return;

    }
}
