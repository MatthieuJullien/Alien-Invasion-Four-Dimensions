using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveEnum
{
    GetAccessCodes, GetWeapons, OpenPortal, GetJumpBooster, SurchargeReactor, EscapeStation
}

[System.Serializable]
public class NewObjectiveCompletedEvent : UnityEvent<ObjectiveEnum> { }

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private Objective[] objectives;


    [SerializeField] private GameObject portalPrefab1;
    [SerializeField] private GameObject alienQueenPortal1;
    [SerializeField] private GameObject portalPrefab2;
    [SerializeField] private GameObject alienQueenPortal2;

    private NewObjectiveCompletedEvent _completedEvent = new NewObjectiveCompletedEvent();

    public NewObjectiveCompletedEvent CompletedEvent { get => _completedEvent; }

    private int _currentObjectiveIndex;

    public Vector3 CurrentObjectivePosition { get => objectives[_currentObjectiveIndex].Position; }

    private void Start()
    {
        _currentObjectiveIndex = 0;
        objectives[0].Unlock();

        alienQueenPortal1.SetActive(false);
        alienQueenPortal2.SetActive(false);
    }

    public bool CompleteObjective(ObjectiveEnum objectiveToComplete)
    {
        if (objectiveToComplete != objectives[_currentObjectiveIndex].Label)
            return false;
        UnlockNextObjective();
        return true;
    }

    public void UnlockNextObjective()
    {
        var currentObjective = objectives[_currentObjectiveIndex];
        currentObjective.Complete();
        CompletedEvent.Invoke(currentObjective.Label);

        _currentObjectiveIndex++;
        objectives[_currentObjectiveIndex].Unlock();
    }

    public bool IsObjectiveCompleted(ObjectiveEnum objectiveLabel)
    {
        foreach (var obj in objectives)
        {
            if (obj.Label == objectiveLabel && obj.IsCompleted)
            {
                return true;
            }
        }
        return false;
    }

    public void OpenPortal()
    {
        alienQueenPortal1.SetActive(true);
        alienQueenPortal2.SetActive(true);
        var vfx = portalPrefab1.GetComponentsInChildren<ParticleSystem>();
        foreach (var effect in vfx)
        {
            effect.Play();
        }
        vfx = portalPrefab2.GetComponentsInChildren<ParticleSystem>();
        foreach (var effect in vfx)
        {
            effect.Play();
        }
    }

    public void WarpToObjective()
    {
        var pos = CurrentObjectivePosition;
        pos += Vector3.up * 5f;
        FindObjectOfType<PlayerHandler>().transform.position = pos;
    }
}
