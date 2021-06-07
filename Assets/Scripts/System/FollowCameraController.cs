using UnityEngine;

public class FollowCameraController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float distanceToTarget = 15.0f;
    [SerializeField] private float followSpeed = 3.0f;

    private Vector3 CameraRelativePosition
    {
        get { return targetTransform.position - transform.forward * distanceToTarget; }
    }

    public void Awake()
    {
        transform.position = CameraRelativePosition;
    }

    public void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, CameraRelativePosition, followSpeed * Time.deltaTime);
    }
}