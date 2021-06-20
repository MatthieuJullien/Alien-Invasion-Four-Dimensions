using UnityEngine;
using UnityEngine.Events;

public enum ObjectiveEnum
{
    GetAccessCodes, GetWeapons, OpenPortal, SurchargeReactor, EscapeStation
}

[System.Serializable]
public class NewObjectiveCompletedEvent : UnityEvent<ObjectiveEnum> { }

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private Objective[] objectives;
    private NewObjectiveCompletedEvent _completedEvent = new NewObjectiveCompletedEvent();
    public NewObjectiveCompletedEvent CompletedEvent { get => _completedEvent; }

    private int _currentObjectiveIndex;

    private void Start()
    {
        _currentObjectiveIndex = 0;
        objectives[0].Unlock();
    }

    public void CompleteObjective(ObjectiveEnum objectiveToComplete)
    {
        if (objectiveToComplete != objectives[_currentObjectiveIndex].Label) return;
        UnlockNextObjective();
    }

    private void UnlockNextObjective()
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
}