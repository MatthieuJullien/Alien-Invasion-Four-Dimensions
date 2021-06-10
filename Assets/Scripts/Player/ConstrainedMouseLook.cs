using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LookFollower
{
    public Transform followingTransform;
    public float rotationModifer;
}

public class ConstrainedMouseLook : MouseLook
{
    [SerializeField] private List<LookFollower> lookFollowers;

    public override void LookRotation(CharacterMovement movement, Transform cameraTransform)
    {
        switch (GameManager.Instance.ViewPoint)
        {
            case PlayerViewPoint.Isometric:
                HorizontalLookRotation(movement);
                VerticalLookRotation(cameraTransform);
                break;
            case PlayerViewPoint.TopDown:
                HorizontalLookRotation(movement);
                break;
            case PlayerViewPoint.SideView:
                HorizontalLookRotation(movement);
                VerticalLookRotation(cameraTransform);
                break;
            case PlayerViewPoint.FirstPerson:
                HorizontalLookRotation(movement);
                VerticalLookRotation(cameraTransform);
                break;
            default:
                break;
        }
    }

    private void UpdateFollowers()
    {
        foreach (var follower in lookFollowers)
        {
            float pitch = Input.GetAxis("Mouse Y") * verticalSensitivity * follower.rotationModifer;
            Quaternion pitchRotation = Quaternion.Euler(-pitch, 0.0f, 0.0f);
            follower.followingTransform.localRotation *= pitchRotation;
            if (clampPitch)
                follower.followingTransform.localRotation = ClampPitch(follower.followingTransform.localRotation);
        }
    }


    private void HorizontalLookRotation(CharacterMovement movement)
    {
        var yaw = Input.GetAxis("Mouse X") * lateralSensitivity;

        if (GameManager.Instance.ViewPoint == PlayerViewPoint.SideView)
        {
            if (yaw > 0)
            {
                movement.rotation = Quaternion.LookRotation(Vector3.right);
            }
            else if (yaw < 0)
            {
                movement.rotation = Quaternion.LookRotation(Vector3.left);
            }
        }
        else
        {
            var yawRotation = Quaternion.Euler(0.0f, yaw, 0.0f);
            characterTargetRotation *= yawRotation;

            if (smooth)
            {
                movement.rotation = Quaternion.Slerp(movement.rotation, characterTargetRotation,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                movement.rotation *= yawRotation;
            }
        }

        UpdateCursorLock();
    }

    public void VerticalLookRotation(Transform cameraTransform)
    {
        var pitch = Input.GetAxis("Mouse Y") * verticalSensitivity;
        var pitchRotation = Quaternion.Euler(-pitch, 0.0f, 0.0f);
        cameraTargetRotation *= pitchRotation;
        if (clampPitch)
            cameraTargetRotation = ClampPitch(cameraTargetRotation);

        if (smooth)
        {
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, cameraTargetRotation,
                smoothTime * Time.deltaTime);
        }
        else
        {
            cameraTransform.localRotation *= pitchRotation;
            if (clampPitch)
                cameraTransform.localRotation = ClampPitch(cameraTransform.localRotation);
        }

        UpdateCursorLock();
        UpdateFollowers();
    }
}
