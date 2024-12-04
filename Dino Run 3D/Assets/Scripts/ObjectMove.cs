using UnityEngine;

public class ObjectMove : MonoBehaviour
{
    public float speed = 5f; // How high the player can jump

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        // Move the object
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
