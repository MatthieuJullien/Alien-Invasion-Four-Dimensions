using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform rootObjectiveMarkerTransform;
    [SerializeField] private MeshRenderer objectiveMarkerMeshRenderer;

    private bool orientAsWorld;
    private ObjectiveManager objectiveManager;

    public void Start()
    {
        objectiveManager = ObjectiveManager.Instance;
        transform.forward = -Vector3.up;
        ToggleViewpoint(GameManager.Instance.IsCurrentViewFPS);
        UpdateMinimapRotation();
    }

    public void LateUpdate()
    {
        UpdateMinimapRotation();
        UpdateObjectiveMarker();
    }

    private void UpdateMinimapRotation()
    {
        if (orientAsWorld)
        {
            transform.forward = -Vector3.up;
        }
    }

    private void UpdateObjectiveMarker()
    {
        Vector3 playerPosition = playerTransform.position;
        var objPos = objectiveManager.CurrentObjectivePosition;

        if (Vector3.Distance(playerPosition, objPos) < 10f)
        {
            objectiveMarkerMeshRenderer.enabled = false;
        }
        else
        {
            objectiveMarkerMeshRenderer.enabled = true;
            rootObjectiveMarkerTransform.LookAt(new Vector3(objPos.x, playerPosition.y, objPos.z));
        }
    }

    public void ToggleViewpoint(bool isFPS)
    {
        orientAsWorld = !isFPS;
        if (orientAsWorld)
        {
            transform.forward = -Vector3.up;
        }
        else
        {
            transform.forward = playerTransform.forward;
            transform.Rotate(Vector3.right, 90f);
        }
    }
}
