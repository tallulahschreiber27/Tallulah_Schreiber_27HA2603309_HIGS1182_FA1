using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Tracking")]
    [SerializeField] private Transform target; // Drag your Player_Drone here

    [Header("Smoothing settings")]
    [Tooltip("Lower values mean smoother, lazier tracking. Higher values mean faster tracking.")]
    [SerializeField] private float smoothSpeed = 5f;

    // Stores the distance between camera and player
    private Vector3 offset;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: No target assigned in the Inspector!");
            return;
        }

        // Calculate the initial distance offset automatically at startup
        offset = transform.position - target.position;
    }

    // LateUpdate runs after regular Update, preventing camera jitter
    private void LateUpdate()
    {
        if (target != null)
        {
            FollowTarget(); // Section B requirement: Single-responsibility custom method
        }
    }

    /// <summary>
    /// Smoothly interpolates the camera's position to track the player drone.
    /// </summary>
    private void FollowTarget()
    {
        // Define our ideal destination position
        Vector3 desiredPosition = target.position + offset;

        // Frame-rate independent smoothing using Vector3.Lerp (Section B requirement)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
