using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [SerializeField] protected LayerMask triggerLayers;
    [SerializeField] protected UnityEvent enterAction;
    [SerializeField] protected UnityEvent exitAction;
    //[SerializeField] private UnityEvent stayAction;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (enterAction == null)
            return;

        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) == otherLayerMask)
        {
            enterAction.Invoke();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (exitAction == null)
            return;

        int otherLayerMask = 1 << other.gameObject.layer;
        if ((triggerLayers.value & otherLayerMask) == otherLayerMask)
        {
            exitAction.Invoke();
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (stayAction == null)
    //        return;

    //    int otherLayerMask = 1 << other.gameObject.layer;
    //    if ((layerMask.value & otherLayerMask) != 0)

    //    {
    //        stayAction.Invoke();
    //    }
    //}
}
