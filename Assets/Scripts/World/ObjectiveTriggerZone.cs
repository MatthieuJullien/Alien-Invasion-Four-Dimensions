using UnityEngine;

public class ObjectiveTriggerZone : TriggerZone
{
    [SerializeField] private ObjectiveEnum objectiveLabel;

    protected new void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) != otherLayerMask) return;

        if (ObjectiveManager.Instance.CompleteObjective(objectiveLabel))
        {
            enterAction.Invoke();
        }
    }
}
