using UnityEngine;

public class ObjectiveTriggerZone : TriggerZone
{
    [SerializeField] private ObjectiveEnum objectiveLabel;
    [SerializeField] private ObjectiveTriggerZone[] sameObjectiveTriggers;

    private bool delete = false;
    private float shrinkFactor = 1f;

    protected new void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) != otherLayerMask) return;

        if (ObjectiveManager.Instance.CompleteObjective(objectiveLabel))
        {
            enterAction.Invoke();
            delete = true;
            foreach (var trigger in sameObjectiveTriggers)
            {
                trigger.delete = true;
            }
        }
    }

    private void Update()
    {
        if (delete)
        {
            shrinkFactor -= Time.deltaTime;
            if (shrinkFactor < 0.02f)
                Destroy(this);
            transform.localScale *= shrinkFactor;
        }
    }
}
