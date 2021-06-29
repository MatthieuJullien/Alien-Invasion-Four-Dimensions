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
            if (yaw > 1f)
            {
                movement.rotation = Quaternion.LookRotation(Vector3.right);
            }
            else if (yaw < -1f)
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

        /*
        if (GameManager.Instance.ViewPoint == PlayerViewPoint.FirstPerson)
        {
            var yaw = Input.GetAxis("Mouse X") * lateralSensitivity;
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
        else // not FirstPerson viewpoint
        {
            Camera cam = GameManager.Instance.CurrentCamera;
            FollowCameraController followCam = cam.GetComponent<FollowCameraController>();
            float dist = followCam.DistanceToTarget;

            Vector3 direction = cam.ScreenPointToRay(Input.mousePosition).direction;
            Vector3 cursorPosition = cam.transform.position + direction * dist;

            if (GameManager.Instance.ViewPoint == PlayerViewPoint.SideView)
            {
                if (cursorPosition.x > transform.position.x)
                {
                    movement.rotation = Quaternion.LookRotation(Vector3.right);
                }
                else if (cursorPosition.x < transform.position.x)
                {
                    movement.rotation = Quaternion.LookRotation(Vector3.left);
                }
            }
            else // topdown or iso
            {
                cursorPosition = new Vector3(cursorPosition.x, transform.position.y, cursorPosition.z);
                movement.rotation = Quaternion.Slerp(movement.rotation, Quaternion.LookRotation(cursorPosition - transform.position), Time.deltaTime * 7f);
                //transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
                //movement.rotation = Quaternion.LookRotation();
            }

            //UpdateCursorLock();
        }
        */
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

        //UpdateCursorLock();
        UpdateFollowers();
    }
}
