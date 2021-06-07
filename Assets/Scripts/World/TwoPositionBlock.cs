using UnityEngine;

public class TwoPositionBlock : MonoBehaviour
{
    [SerializeField] private float movingSpeed = 3f;
    [SerializeField] private Transform transformA;
    [SerializeField] private Transform transformB;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private bool startInA = true;
    [SerializeField] private bool isLooping = false;

    private Transform target;
    private bool isMoving = false;

    private void Start()
    {
        if (startInA)
        {
            modelTransform.position = transformA.position;
            target = transformB;
        }
        else
        {
            modelTransform.position = transformB.position;
            target = transformA;
        }
        isMoving = isLooping;
    }

    public void SwitchTarget()
    {
        target = (target == transformA) ? transformB : transformA;
    }

    public void MoveToTarget()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (isMoving)
        {
            Vector3 movement = Vector3.MoveTowards(modelTransform.position, target.position, movingSpeed * Time.deltaTime);
            modelTransform.position = movement;
            if (movement == target.position)
            {
                isMoving = isLooping;
                SwitchTarget();
            }
        }
    }
}
