using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [SerializeField] private Objective[] objectives;
    private int _currentObjectiveIndex;

    private void Start()
    {
        _currentObjectiveIndex = 0;
        objectives[0].Unlock();
    }

    public void UnlockNextObjective()
    {
        objectives[_currentObjectiveIndex].Complete();
        _currentObjectiveIndex++;
        objectives[_currentObjectiveIndex].Unlock();
    }
}
