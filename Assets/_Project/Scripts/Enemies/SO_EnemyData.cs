using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "AI/EnemyData")]
public class SO_EnemyData : ScriptableObject
{
    [Header("Idle States")]
    public float viewAngle = 130f;
    public float viewDistance = 10f;
    public float rotationInterval = 5f;
    public float lookAroundTime = 3f;
    public float viewWidth = 2f;

    [Header("After Chase State (When Player Is Lost)")]
    public float rotationSpeed = 5f;
    public float waitTimer = 1f;
}
