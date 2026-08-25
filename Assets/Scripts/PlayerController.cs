using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    [Header("Shooting Settings")]
    public Transform firePoint;     // Drag your empty Fire_Point object here
    public float attackRange = 50f; // How far the laser raycast can travel

    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        // SECTION B: Cache components in Start() to save performance
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Verify the component exists safely
        if (rb == null)
        {
            Debug.LogError("PlayerController: Missing a Rigidbody component on this object!");
        }
    }

    void Update()
    {
        // SECTION B: Custom methods keep our Update loop clean and structured
        HandleMovementInput();
        HandleRotationInput();
        HandleShootingInput();
    }

    // CUSTOM METHOD 1: Processes WASD / Arrow keys for flight movement
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Combine into a flat 3D movement vector
        Vector3 movementDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Unity 6 standard: Use linearVelocity instead of velocity
        rb.linearVelocity = movementDirection * moveSpeed;
    }

    // CUSTOM METHOD 2: Spins the drone to face the mouse smoothly
    private void HandleRotationInput()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        // FIX: Creates the reference plane exactly at the drone's current height
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(cameraRay, out float rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);

            // Keep Y position flat so the drone doesn't flip up or wobble down
            Vector3 targetLookPosition = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);

            transform.LookAt(targetLookPosition);
        }
    }

    // CUSTOM METHOD 3: Listens for left-clicks to trigger weapon fire
    private void HandleShootingInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            FireRaycastLaser();
        }
    }

    // CUSTOM METHOD 4: Casts an instant laser beam forward
    private void FireRaycastLaser()
    {
        // SECTION B: Mandatory trace debugging log
        Debug.Log("Player Fired Laser Weapon");

        // Safety check to prevent crashes if Fire Point isn't connected in Inspector
        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is not assigned on the PlayerController!");
            return;
        }

        // SECTION A: Core raycast logic shooting straight forward from firePoint tip
        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hitInfo, attackRange))
        {
            Debug.Log($"Laser struck an object named: {hitInfo.collider.name}");

            // Check if the raycast hit an asteroid tag target
            if (hitInfo.collider.CompareTag("Asteroid"))
            {
                // Instantly clean up the hit asteroid from the game world
                Destroy(hitInfo.collider.gameObject);
                Debug.Log("Asteroid successfully neutralized by Laser!");
            }
        }
    }
}
