using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform Target;
    Vector3 newPosition;

    private void LateUpdate()
    {
        newPosition = new Vector3(Target.transform.position.x, Target.transform.position.y-5, transform.position.z);
        transform.position = newPosition;
    }
}
