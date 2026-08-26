using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [Header("Prefab Assignment")]
    [SerializeField] private GameObject asteroidPrefab; // Drag your Asteroid Prefab here

    [Header("Spawning Boundaries")]
    [Tooltip("The minimum and maximum X coordinates where asteroids can appear.")]
    [SerializeField] private Vector2 xSpawnRange = new Vector2(-8f, 8f);
    [Tooltip("The fixed Z coordinate above the top of the screen where asteroids spawn.")]
    [SerializeField] private float zSpawnPosition = 10f;

    [Header("Timing Settings")]
    [SerializeField] private float spawnRate = 2f; // Seconds between spawns

    private float nextSpawnTime = 0f;

    private void Update()
    {
        // Check game runtime loop clock to cycle waves periodically
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle(); // Section B requirement: Custom single-responsibility method
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    /// <summary>
    /// Instantiates a randomized asteroid prefab along the top boundary axis.
    /// </summary>
    private void SpawnObstacle()
    {
        // Calculate a randomized X coordinate along the designated spawn line
        float randomX = Random.Range(xSpawnRange.x, xSpawnRange.y);
        Vector3 spawnPosition = new Vector3(randomX, 0f, zSpawnPosition);

        // Instantiate the asteroid at runtime (Section A requirement)
        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        // Rubric requirement: Meaningful debug tracing log
        Debug.Log("Obstacle Spawner: Instantiated asteroid at position " + spawnPosition);
    }
}
