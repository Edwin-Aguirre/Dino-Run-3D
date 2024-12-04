using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> prefabs;        // List of prefabs to pool
    public int poolSize = 10;                // Size of the pool
    private List<GameObject> pool;           // List to hold pooled objects

    void Awake()
    {
        // Initialize the pool
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabToUse = prefabs[Random.Range(0, prefabs.Count)]; // Randomly select a prefab
            GameObject obj = Instantiate(prefabToUse);
            obj.SetActive(false); // Disable the object initially
            pool.Add(obj);
        }
    }

    // Method to get an object from the pool
    public GameObject GetPooledObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy) // Check if the object is inactive
            {
                return obj; // Return the inactive object
            }
        }
        return null; // Return null if no objects are available
    }
}
