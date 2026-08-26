using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Prefab Assignment")]
    [SerializeField] private GameObject collectiblePrefab; // Drag your universal Collectible Prefab here

    [Header("Spawning Boundaries")]
    [SerializeField] private Vector2 xSpawnRange = new Vector2(-8f, 8f);
    [SerializeField] private float zSpawnPosition = 10f;

    [Header("Timing Settings")]
    [SerializeField] private float spawnRate = 3f;

    private float nextSpawnTime = 0f;

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnCollectible(); // Section B requirement: Custom single-responsibility method
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void SpawnCollectible()
    {
        float randomX = Random.Range(xSpawnRange.x, xSpawnRange.y);
        Vector3 spawnPosition = new Vector3(randomX, 0f, zSpawnPosition);

        Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Collectible Manager: Spawned an item prefab at " + spawnPosition);
    }
}
