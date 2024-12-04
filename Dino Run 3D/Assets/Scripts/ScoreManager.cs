using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject[] numberModels; // Assign your number models in the inspector
    private float startTime;
    private int currentScore;
    private int highScore;
    private bool isGameActive;
    public bool hasStarted; // Track if scoring has started

    void Start()
    {
        startTime = Time.time; // Record the start time
        currentScore = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0); // Load high score from PlayerPrefs
        isGameActive = false; // Start inactive until spacebar is pressed
        hasStarted = false; // Scoring has not started yet
    }

    void Update()
    {
        if (isGameActive)
        {
            // Update the score based on elapsed time
            currentScore = Mathf.FloorToInt(Time.time - startTime);
            UpdateScore();
            
            // Check if current score exceeds high score
            if (currentScore > highScore)
            {
                highScore = currentScore; // Update high score
                PlayerPrefs.SetInt("HighScore", highScore); // Save new high score
            }
        }
    }

    public void StopScore()
    {
        isGameActive = false; // Stop updating the score
    }

    public void StartScore()
    {
        isGameActive = true; // Start updadting the score
        startTime = Time.time; // Reset start time when scoring starts
    }

    public void ResetScore()
    {
        isGameActive = true; // Reset game active to true, if needed for respawn or new game
        startTime = Time.time; // Reset the start time
        currentScore = 0; // Reset the current score
        UpdateScore(); // Update the displayed score
    }

    void UpdateScore()
    {
        // Clear old score models
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Display current score
        DisplayScore(currentScore);

        // Optionally, display high score as well
        DisplayScore(highScore, new Vector3(0, -2.5f, 0)); // Adjust position for high score if needed
    }

    void DisplayScore(int score, Vector3 positionOffset = default)
    {
        string scoreString = score.ToString();
        float offsetX = 1.5f; // Adjust this value for spacing between digits
        Vector3 startPosition = transform.position + positionOffset; // Use the position of the parent GameObject

        for (int i = 0; i < scoreString.Length; i++)
        {
            int digit = int.Parse(scoreString[i].ToString());
            GameObject numberModel = Instantiate(numberModels[digit], startPosition + new Vector3(i * offsetX, 0, 0), Quaternion.identity);
            numberModel.transform.parent = transform; // Set parent for easier management
            numberModel.gameObject.transform.Rotate(0, 180, 0); // Rotate numbers to correct position
        }
    }
}
