using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign the plane GameObject to this in the Inspector
    public float smoothSpeed = 0.125f; // Adjust this value for desired smoothing
    public Vector3 offset; // Set a camera offset from the plane (optional)

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.LookAt(target); // Make the camera look at the target plane
        }
    }
}
