using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTriggerZone : TriggerZone
{
    [SerializeField] private ObjectiveEnum objectiveLabel;

    protected new void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) == otherLayerMask)
        {
            enterAction.Invoke();
            ObjectiveManager.Instance.CompleteObjective(objectiveLabel);
        }
    }
}
