using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Base speed at which the asteroid drifts down the screen.")]
    [SerializeField] private float driftSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        // Section B requirement: Cache the Rigidbody component for performance optimization
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Turn off gravity so it stays on our flat horizontal gameplay plane
            rb.useGravity = false;

            // 1. Randomize Speed: Gives each rock a unique pace (between 70% and 130% of base driftSpeed)
            float randomSpeed = Random.Range(driftSpeed * 0.7f, driftSpeed * 1.3f);

            // 2. Randomize Direction: Adds a tiny bit of random horizontal slide along the X-axis
            float randomHorizontalDrift = Random.Range(-1.5f, 1.5f);

            // Combine into a unique vector pointing down the screen along the horizontal Z-axis (-Z)
            Vector3 uniqueDirection = new Vector3(randomHorizontalDrift, 0f, -1f).normalized;
            rb.linearVelocity = uniqueDirection * randomSpeed; // Unity 6 production standard

            // 3. Randomize Spin: Adds a fun, organic tumbling rotation through space
            float randomSpin = Random.Range(-50f, 50f);
            rb.angularVelocity = new Vector3(0f, randomSpin, 0f);
        }
        else
        {
            Debug.LogError($"Asteroid Error: Rigidbody component is missing on {gameObject.name}!");
        }
    }

    /// <summary>
    /// Handles player crash detection using Unity trigger fields.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log(" 3D Asteroid hit the Player!");

            // TODO: Call your GameManager to load the Game Over screen here!

            Destroy(other.gameObject); // Wipe out the player drone
            Destroy(gameObject);       // Destroy this asteroid on impact
        }
    }

    /// <summary>
    /// Section B requirement: Correct implementation of boundary exit cleanup triggers
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // Clean up when the asteroid safely exits your large screen boundary box
        if (other.CompareTag("GameplayBoundary"))
        {
            Debug.Log($"[Cleanup] '{gameObject.name}' left the arena and cleanly self-destructed.");
            Destroy(gameObject);
        }
    }
}
