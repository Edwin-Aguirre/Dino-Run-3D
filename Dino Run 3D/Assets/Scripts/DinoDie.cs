using UnityEngine;
using UnityEngine.SceneManagement;

public class DinoDie : MonoBehaviour
{
    [SerializeField]
    private AudioSource dieAudio; // Reference to the AudioSource

    private Animator animator; // Reference to the Animator
    private DinoJump dinoJump;
    private ScoreManager scoreManager;
    private DinoUI dinoUI;

    public bool hasDied = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        dinoJump = GetComponent<DinoJump>(); // Get the DinoJump Script
        scoreManager = FindAnyObjectByType<ScoreManager>();
        dinoUI = FindAnyObjectByType<DinoUI>();
        Time.timeScale = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Enemy" tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            Death();
        }
    }

    private void PlayDieSound()
    {
        if (dieAudio != null)
        {
            dieAudio.Play(); // Play the die sound
        }
        else
        {
            Debug.LogWarning("Die AudioSource not assigned!");
        }
    }

    private void Death()
    {
        PlayDieSound();

        hasDied = true;

        // Stop dino from jumping
        dinoJump.jumpForce = 0;

        // Pause game when dino dies
        Time.timeScale = 0;

        // Stop score
        scoreManager.StopScore();

        // Show UI
        dinoUI.ShowUI();

        // Update the Animator parameter based on the dead state
        animator.SetBool("isDead", true);
    }
}
