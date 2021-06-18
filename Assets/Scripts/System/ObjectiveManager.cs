using UnityEngine;

public enum ObjectiveEnum
{
    GetAccessCodes, GetWeapons, OpenPortal, SurchargeReactor, EscapeStation
}

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private Objective[] objectives;
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
        objectives[_currentObjectiveIndex].Complete();
        _currentObjectiveIndex++;
        objectives[_currentObjectiveIndex].Unlock();
    }
}
