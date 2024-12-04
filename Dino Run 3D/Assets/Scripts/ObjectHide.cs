using UnityEngine;

public class ObjectHide : MonoBehaviour
{
    public float disableThreshold = -450f; // Set this to the Y position where the obstacle should be disabled

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // Check if the obstacle has fallen below the threshold
        if (transform.position.x < disableThreshold)
        {
            gameObject.SetActive(false); // Disable the obstacle
        }
    }
}
