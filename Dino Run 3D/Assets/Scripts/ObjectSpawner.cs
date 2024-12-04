using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public ObjectPool objectPool; // Reference to the object pool
    public float spawnInterval = 2f; // Time between spawns
    public float spawnMaxY = 2f;    // Max Range for Y position
    public float spawnMinY = 2f;    // Min Range for Y position
    public float spawnZ = 0f;       // Range for Z position
    public float spawnDistance = 10f;  // Distance from the camera to spawn

    private float timer;

    void Start()
    {
        timer = spawnInterval; // Start the timer
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0; // Reset timer
        }
    }

    void SpawnObstacle()
    {
        GameObject obstacle = objectPool.GetPooledObject(); // Get an object from the pool
        
        if (obstacle != null)
        {
            // Calculate random Y position within range
            float randomY = Random.Range(spawnMinY, spawnMaxY);
            
            // Set spawn position
            Vector3 spawnPosition = new Vector3(spawnDistance, randomY, spawnZ);
            
            // Activate the obstacle and set its position
            obstacle.transform.position = spawnPosition;
            obstacle.SetActive(true);
        }
    }
}
