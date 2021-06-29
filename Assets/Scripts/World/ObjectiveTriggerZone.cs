using UnityEngine;

public class ObjectiveTriggerZone : TriggerZone
{
    [SerializeField] private ObjectiveEnum objectiveLabel;
    private bool delete = false;
    private float shrinkFactor = 1f;

    protected new void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) != otherLayerMask) return;

        print("enter");
        if (ObjectiveManager.Instance.CompleteObjective(objectiveLabel))
        {
            print("complete ovjective");
            enterAction.Invoke();
            delete = true;
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
