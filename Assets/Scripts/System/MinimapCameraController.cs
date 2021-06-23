using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private bool orientAsWorld;

    public void Start()
    {
        transform.forward = -Vector3.up;
        ToggleViewpoint(GameManager.Instance.IsCurrentViewFPS);
        UpdateRotation();
    }

    public void LateUpdate()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (orientAsWorld)
        {
            transform.forward = -Vector3.up;
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
