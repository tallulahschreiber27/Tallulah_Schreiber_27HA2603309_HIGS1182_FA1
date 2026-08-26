using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vector2 xBounds = new Vector2(-8f, 8f);
    [SerializeField] private Vector2 zBounds = new Vector2(-4f, 4f);

    [Header("Aiming Settings")]
    [Tooltip("Controls how fast the ship rotates towards the mouse. Lower values make it smoother.")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Aiming Restrictions")]
    [Tooltip("Maximum degrees the drone can pivot left or right from straight forward.")]
    [SerializeField] private float maxAimAngle = 60f;

    [Header("Center Return Settings")]
    [Tooltip("Distance threshold from the drone to your cursor. If the mouse is closer than this, the drone snaps straight forward.")]
    [SerializeField] private float centerDeadzone = 2.0f;

    [Header("Shooting Settings")]
    [SerializeField] private float attackRange = 50f;
    [SerializeField] private LayerMask asteroidLayer;
    [SerializeField] private Transform firePoint;

    // Cached variables for performance and logic tracking (Section B requirements)
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private Quaternion targetRotation;

    private void Start()
    {
        // Cache references at startup for performance optimization
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        if (rb == null)
        {
            Debug.LogError("PlayerController: Missing required Rigidbody component!");
        }

        // Initialize mouse tracking positions at startup
        lastMousePosition = Input.mousePosition;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        // Separated single-responsibility custom methods (Section B requirement)
        HandleMovementInput();
        HandleMouseAiming();
        HandleShootingInput();
    }

    /// <summary>
    /// Reads player movement input and keeps the drone locked inside screen bounds.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Maintain frame-rate independent movement velocity on flat XZ plane
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * moveSpeed;
        rb.linearVelocity = movement;

        // Clamp positions tightly within your scene boundaries
        float clampedX = Mathf.Clamp(transform.position.x, xBounds.x, xBounds.y);
        float clampedZ = Mathf.Clamp(transform.position.z, zBounds.x, zBounds.y);
        transform.position = new Vector3(clampedX, 0f, clampedZ);
    }

    /// <summary>
    /// Updates rotation goals ONLY when the mouse moves screen coordinates.
    /// </summary>
    private void HandleMouseAiming()
    {
        // GATING CHECK: Only recalculate aiming if the mouse cursor coordinates change
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                Vector3 targetDirection = new Vector3(lookPoint.x, 0f, lookPoint.z) - transform.position;

                // DEADZONE CHECK: Snap straight forward if cursor gets too close
                if (targetDirection.magnitude < centerDeadzone)
                {
                    targetDirection = Vector3.forward;
                }

                if (targetDirection != Vector3.zero)
                {
                    float angle = Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.up);
                    angle = Mathf.Clamp(angle, -maxAimAngle, maxAimAngle);

                    // Update internal destination rotation target
                    targetRotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }

        // Frame-rate independent smoothing towards target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // HARD LOCK: Eliminate any accidental X or Z tilting caused by physics glitches
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    /// <summary>
    /// Checks if the player clicks to fire.
    /// </summary>
    private void HandleShootingInput()
    {
        if (Input.GetButtonDown("Fire1")) // Left Mouse Click
        {
            ExecuteRaycastShoot();
        }
    }

    /// <summary>
    /// Executes the mandatory 3D Raycast to hit and destroy target obstacles.
    /// </summary>
    private void ExecuteRaycastShoot()
    {
        Debug.Log("Player fired a laser bolt.");
        RaycastHit hit;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackRange, asteroidLayer))
        {
            Debug.Log($"Raycast Hit Target: {hit.collider.name}");

            if (hit.collider.CompareTag("Asteroid"))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Asteroid successfully shattered via Raycast!");
            }
        }
    }

    /// <summary>
    /// Section B requirement: Correct use of trigger detection for items
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 1. Collect Fuel Canister
        if (other.CompareTag("Collectible"))
        {
            Collectible item = other.GetComponent<Collectible>();
            if (item != null)
            {
                Debug.Log($"[Collection] Picked up a '{item.GetCollectibleType()}' canister!");

                // CONNECT TO GAMEMANAGER: Tell the manager to add 1 point to the score counter
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(1);
                }

                Destroy(other.gameObject);
            }
        }

        // 2. Crash into Asteroid
        if (other.CompareTag("Asteroid"))
        {
            Debug.Log(" 3D Asteroid hit the Player!");

            // CONNECT TO GAMEMANAGER: Tell the manager to trigger the Game Over scene swap
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }

            Destroy(gameObject);
        }
    }



    /// <summary>
    /// Section B requirement: Correct use of standard physical collision for obstacles
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log(" PHYSICAL IMPACT! The drone smashed into an Asteroid. Game Over!");

            // TODO: Call your GameManager to load the Game Over screen here

            Destroy(gameObject); // Instantly destroy the player drone
        }
    }
}
