using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // the object to follow
    public Vector3 offset;        // offset from the target

    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
