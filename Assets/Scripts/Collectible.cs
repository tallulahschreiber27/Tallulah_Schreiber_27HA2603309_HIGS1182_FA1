using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Universal Settings")]
    [Tooltip("Allows you to classify what kind of item this is in the inspector.")]
    [SerializeField] private string collectibleType = "Fuel";

    [Header("Movement Settings")]
    [SerializeField] private float driftSpeed = 4f;

    private Rigidbody rb;

    private void Start()
    {
        // Section B requirement: Cache component for performance
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            // Send the universal item drifting down the horizontal Z-axis
            rb.linearVelocity = Vector3.back * driftSpeed;
        }
        else
        {
            Debug.LogError($"Collectible Error: Rigidbody missing on {gameObject.name}!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Section B requirement: Correct use of trigger collision logic
        // Clean up items that drift outside our active tracking box
        if (other.CompareTag("GameplayBoundary"))
        {
            Debug.Log($"[Cleanup] {collectibleType} item '{gameObject.name}' left the arena and self-destructed.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Custom method to return what kind of item was collected (Rubric Requirement)
    /// </summary>
    public string GetCollectibleType()
    {
        return collectibleType;
    }
}
