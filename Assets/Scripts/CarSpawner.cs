using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject carPrefab;
    public Transform[] spawnPoints; // Array of spawn positions for different lanes
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    [Header("Car Settings")]
    public float minCarSpeed = 2f;
    public float maxCarSpeed = 5f;

    private float nextSpawnTime;
    private bool isSpawning = true;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (!isSpawning) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnCar();
            ScheduleNextSpawn();
        }
    }

    void SpawnCar()
    {
        if (spawnPoints.Length == 0 || carPrefab == null) return;

        // Choose random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Spawn car
        GameObject car = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);

        // Set random speed
        CarController carController = car.GetComponent<CarController>();
        if (carController != null)
        {
            carController.speed = Random.Range(minCarSpeed, maxCarSpeed);

            // Alternate direction based on lane (even lanes go right, odd go left)
            carController.moveRight = (randomIndex % 2 == 0);

            // Flip sprite if moving left
            if (!carController.moveRight)
            {
                car.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    void ScheduleNextSpawn()
    {
        float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + interval;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        ScheduleNextSpawn();
    }
}