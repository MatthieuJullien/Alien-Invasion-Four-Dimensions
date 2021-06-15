using UnityEngine;

public class MultipleViewPlayerController : BaseCharacterController
{
    public float runSpeedMultiplier = 2.0f;

    [Header("FPS speeds")]
    [SerializeField] private float forwardSpeed = 6.0f;
    [SerializeField] private float backwardSpeed = 4.0f;
    [SerializeField] private float strafeSpeed = 5.0f;

    public Transform CameraPivotTransform { get; private set; }
    public Transform CameraTransform { get; private set; }
    public MouseLook MouseLook { get; private set; }
    public bool Run { get; set; }

    protected virtual void RotateView()
    {
        MouseLook.LookRotation(movement, CameraTransform);
    }

    protected override void UpdateRotation()
    {
        RotateView();
    }

    protected virtual float GetTargetSpeed()
    {
        float targetSpeed = forwardSpeed;

        if (moveDirection.x > 0.0f || moveDirection.x < 0.0f)
            targetSpeed = strafeSpeed;

        if (moveDirection.z < 0.0f)
            targetSpeed = backwardSpeed;

        // forward speed should take precedence
        if (moveDirection.z > 0.0f)
            targetSpeed = forwardSpeed;

        return Run ? targetSpeed * runSpeedMultiplier : targetSpeed;
    }

    protected override void Move()
    {
        Vector3 desiredVelocity = CalcDesiredVelocity();

        var currentFriction = isGrounded ? groundFriction : airFriction;
        var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

        movement.Move(desiredVelocity, speed * runSpeedMultiplier, acceleration, deceleration, currentFriction,
            currentBrakingFriction, !allowVerticalMovement);

        // Jump logic
        Jump();
        MidAirJump();
        UpdateJumpTimer();
    }

    protected override Vector3 CalcDesiredVelocity()
    {
        if (GameManager.Instance.ViewPoint == PlayerViewPoint.TopDown || GameManager.Instance.ViewPoint == PlayerViewPoint.Isometric)
        {
            Vector3 desiredVelocity = base.CalcDesiredVelocity();
            desiredVelocity *= Run ? runSpeedMultiplier : 1f;
            return desiredVelocity;
        }
        else
        {
            speed = GetTargetSpeed();
            return transform.TransformDirection(base.CalcDesiredVelocity());
        }
    }

    protected override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
            pause = !pause;

        int forward = 0;
        int right = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
        {
            forward++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forward--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            right++;
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            right--;
        }

        if (GameManager.Instance.ViewPoint == PlayerViewPoint.SideView)
        {
            jump = (forward == 1) || Input.GetButton("Jump");
            forward = (int)Mathf.Sign(Vector3.Dot(transform.forward, Vector3.right)) * right;
            right = 0;
        }
        else
        {
            jump = Input.GetButton("Jump");
        }

        moveDirection = new Vector3
        {
            x = right,
            y = 0.0f,
            z = forward
        };

        Run = Input.GetButton("Fire3");
    }

    public override void Awake()
    {
        base.Awake();

        MouseLook = GetComponent<MouseLook>();
        CameraPivotTransform = transform.Find("Camera_Pivot");
        if (CameraPivotTransform == null)
        {
            Debug.LogError($"BaseFPSController: No 'Camera_Pivot' found. Please parent a transform gameobject to '{name}' game object.");
        }

        Camera cam = GetComponentInChildren<Camera>();
        CameraTransform = cam.transform;
        MouseLook.Init(transform, CameraTransform);
    }
}
