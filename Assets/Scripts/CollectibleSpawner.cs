using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject coinPrefab;

    [Header("Spawn Area")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -3f;
    public float maxY = 3f;

    [Header("Spawn Settings")]
    public int coinCount = 10;
    public float minDistanceBetweenCoins = 1.5f; // Minimum distance between coins
    public int maxSpawnAttempts = 50; // Max attempts to find valid position per coin

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnCollectibles();
    }

    public void SpawnCollectibles()
    {
        // Clear existing collectibles
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins) Destroy(coin);

        // Clear stored positions
        spawnedPositions.Clear();

        // Spawn coins with proper spacing
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 spawnPos = GetValidRandomPosition();
            if (spawnPos != Vector3.zero) // Valid position found
            {
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
                spawnedPositions.Add(spawnPos);
            }
        }
    }

    Vector3 GetValidRandomPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 randomPos = GetRandomPosition();

            // Check if position is far enough from all previously spawned coins
            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }
        }

        // If no valid position found after max attempts, return zero vector
        Debug.LogWarning("Could not find valid spawn position after " + maxSpawnAttempts + " attempts");
        return Vector3.zero;
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check distance from all previously spawned positions
        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            float distance = Vector3.Distance(position, spawnedPos);
            if (distance < minDistanceBetweenCoins)
            {
                return false; // Too close to another coin
            }
        }
        return true; // Position is valid
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector3(x, y, 0);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize spawn area in editor
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);

        // Visualize minimum distance circles
        if (Application.isPlaying && spawnedPositions.Count > 0)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            foreach (Vector3 pos in spawnedPositions)
            {
                Gizmos.DrawWireSphere(pos, minDistanceBetweenCoins);
            }
        }
    }
}