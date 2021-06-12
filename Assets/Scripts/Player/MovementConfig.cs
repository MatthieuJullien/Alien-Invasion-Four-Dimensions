using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementConfig", order = 1)]
public class MovementConfig : ScriptableObject
{
    public float baseJumpHeight = 1.8f;
    public float extraJumpPower = 25f;
    public float speed = 8f;
    public float runSpeedMultiplier = 1.6f;
    public float airControl = 0.3f;
}