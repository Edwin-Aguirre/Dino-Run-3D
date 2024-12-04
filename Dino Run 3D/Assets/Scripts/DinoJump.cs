using UnityEngine;

public class DinoJump : MonoBehaviour
{
    [SerializeField]
    private AudioSource jumpAudio; // Reference to the AudioSource
    [SerializeField]
    private AudioSource landAudio; // Reference to the AudioSource

    [SerializeField]
    public float jumpForce = 5f; // How high the player can jump

    private bool isGrounded; // To check if the player is on the ground
    private bool hasJumped; // To check if the player has jumped before

    private Rigidbody rb; // Reference to the Rigidbody
    private Animator animator; // Reference to the Animator
    private DinoManager dinoManager; // Reference to the DinoManager

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>(); // Get the Animator component
        dinoManager = FindAnyObjectByType<DinoManager>(); // Get the DinoManager component
        hasJumped = false; // Initialize hasJumped to false
    }

    void Update()
    {
        // Check for jump input and if the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded || Input.GetMouseButtonDown(0) && isGrounded)
        {
            if(!dinoManager.isPaused)
            {
                Jump();
            }
        }

        // Update the Animator parameter based on the jumping state
        animator.SetBool("isJumping", !isGrounded);
    }

    void Jump()
    {
        // Apply a force upwards for the jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Set grounded to false after jumping
        hasJumped = true; // Mark that the player has jumped

        // Play the jump sound only if the player's jump force is greater than 0
        if (jumpForce > 0)
        {
            jumpAudio.Play(); // Play the jump sound
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player has collided with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Reset grounded state when on the ground

            // Play the land sound only if the player has jumped before
            if (hasJumped)
            {
                landAudio.Play(); // Play the land sound
            }
        }
    }
}
