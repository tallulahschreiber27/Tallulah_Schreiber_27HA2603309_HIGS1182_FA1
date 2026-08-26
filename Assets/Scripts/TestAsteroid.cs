using UnityEngine;

public class TestAsteroid : MonoBehaviour
{
    [Header("Experiment Settings")]
    [SerializeField] private float driftSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            // Mirror the exact downward Z-axis drift your canisters use
            rb.linearVelocity = Vector3.back * driftSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Keep your clean boundary tracking active
        if (other.CompareTag("GameplayBoundary"))
        {
            Destroy(gameObject);
        }
    }
}
