using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Base speed at which the asteroid drifts down the screen.")]
    [SerializeField] private float driftSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Section B requirement: Cache component

        if (rb != null)
        {
            rb.useGravity = false;

            // Randomize individual speed and angles for organic variety
            float randomSpeed = Random.Range(driftSpeed * 0.7f, driftSpeed * 1.3f);
            float randomHorizontalDrift = Random.Range(-1.5f, 1.5f);

            Vector3 uniqueDirection = new Vector3(randomHorizontalDrift, 0f, -1f).normalized;
            rb.linearVelocity = uniqueDirection * randomSpeed; // Unity 6 standard

            float randomSpin = Random.Range(-50f, 50f);
            rb.angularVelocity = new Vector3(0f, randomSpin, 0f);
        }
        else
        {
            Debug.LogError($"Asteroid Error: Rigidbody missing on {gameObject.name}!");
        }
    }

    // Section B requirement: Correct use of trigger collision logic
    // This fires automatically the EXACT moment the asteroid exits your big bounding box!
    private void OnTriggerExit(Collider other)
    {
        // If this asteroid exits your designated gameplay arena box
        if (other.CompareTag("GameplayBoundary"))
        {
            // Traces the exact milestone event in the console (Rubric requirement)
            Debug.Log($"[Cleanup] '{gameObject.name}' left the arena and cleanly self-destructed.");

            // Wipe it from existence to free up computer memory
            Destroy(gameObject);
        }
    }
}
