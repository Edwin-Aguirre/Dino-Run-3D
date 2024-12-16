using UnityEngine;

public class DinoManager : MonoBehaviour
{
    private ScoreManager scoreManager;
    private DinoDie dinoDie;

    [SerializeField]
    private GameObject title;

    [SerializeField]
    private GameObject pause;

    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private GameObject fullScreen;

    [SerializeField]
    private GameObject windowed;

    [SerializeField]
    private GameObject resetScore;

    [SerializeField]
    private GameObject[] hideObjects;

    // The predefined positions the arrow can snap to
    public float[] snapPositions = { 0f, -5f, -10f };  // Customize these positions as needed
    private int currentSnapIndex = 0;  // Keep track of the current snap position

    private bool isMoving = false;  // Flag to prevent multiple movement inputs at the same time
    public bool isPaused = false;
    public bool canAdjustVolume = false;

    // Timer variables for holding the Enter key
    private float enterHoldTime = 0f;  // Time the player has held Enter
    private bool isHoldingEnter = false;  // Flag to track if Enter key is being held
    private bool gameWindowMode = false;
    private bool resetScoreMode = false;

    // Store the resolution and window size for windowed mode
    private int windowedWidth;
    private int windowedHeight;
    private bool isWindowedMode = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        dinoDie = FindAnyObjectByType<DinoDie>();

        title.SetActive(true);
        pause.SetActive(false);
        isPaused = false;

        foreach (var objects in hideObjects)
        {
            objects.SetActive(false);
        }

        // Set the arrow's initial position to the first snap position
        arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, snapPositions[currentSnapIndex], arrow.transform.localPosition.z);

        // Store the initial windowed resolution
        windowedWidth = Screen.width;
        windowedHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!scoreManager.hasStarted && !isPaused)
            {
                scoreManager.StartScore();
                scoreManager.hasStarted = true;
                title.SetActive(false);

                foreach (var objects in hideObjects)
                {
                    objects.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
                if (!scoreManager.hasStarted)
                {
                    title.SetActive(true);
                }
            }
            else
            {
                if (dinoDie.hasDied == false)
                {
                    Pause();
                }
            }
        }

        // Continuously check for the arrow movement when the game is paused
        if (isPaused)
        {
            MenuIndicator();
            CheckForVolumeAdjustment();
            CheckForFullscreenToggle();
            CheckForResetScore();
        }

        // Track the Enter key holding time
        if (isHoldingEnter)
        {
            enterHoldTime += Time.unscaledDeltaTime;  // Use unscaledDeltaTime to prevent time scale issues
            if (enterHoldTime >= 3f && gameWindowMode)  // 3 seconds reached
            {
                ToggleFullscreen();
                isHoldingEnter = false;  // Reset flag after action is triggered
            }
            if (enterHoldTime >= 3f && resetScoreMode)
            {
                ToggleResetScore();
                isHoldingEnter = false;  // Reset flag after action is triggered
            }
        }

        // Reset the timer if Enter key is released
        if (Input.GetKeyUp(KeyCode.Return))
        {
            isHoldingEnter = false;
            enterHoldTime = 0f;
        }
    }

    void Pause()
    {
        pause.SetActive(true);
        title.SetActive(false);
        fullScreen.SetActive(!Screen.fullScreen);
        windowed.SetActive(Screen.fullScreen);
        resetScore.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Resume()
    {
        pause.SetActive(false);
        arrow.SetActive(false);
        fullScreen.SetActive(false);
        windowed.SetActive(false);
        resetScore.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Reset the position of the arrow if needed
        arrow.transform.localPosition = new Vector3(4, arrow.transform.localPosition.y, arrow.transform.localPosition.z);  // Set to a known position
    }

    void MenuIndicator()
    {
        // Make sure the arrow is visible when the menu is active
        arrow.SetActive(true);

        if (isMoving) return;  // Prevent further input if the arrow is already moving to a snap position

        // Check for key press to snap to new position
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // If not at the bottom position, move to the next one
            if (currentSnapIndex < snapPositions.Length - 1)
            {
                currentSnapIndex++;
                StartCoroutine(SnapToPosition(snapPositions[currentSnapIndex]));
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // If not at the top position, move to the previous one
            if (currentSnapIndex > 0)
            {
                currentSnapIndex--;
                StartCoroutine(SnapToPosition(snapPositions[currentSnapIndex]));
            }
        }
    }

    // Coroutine to smoothly snap to a new position
    private System.Collections.IEnumerator SnapToPosition(float targetY)
    {
        isMoving = true;

        float startY = arrow.transform.localPosition.y;
        float elapsedTime = 0f;
        float duration = 0.2f;  // Duration of the snapping animation

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;  // Use unscaled time for smooth movement
            float newY = Mathf.Lerp(startY, targetY, elapsedTime / duration);
            arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, newY, arrow.transform.localPosition.z);
            yield return null;
        }

        // Ensure the final position is set precisely
        arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, targetY, arrow.transform.localPosition.z);

        isMoving = false;  // Allow the arrow to be moved again
    }

    // Check if the arrow is at a specific position
    private void CheckForVolumeAdjustment()
    {
        // If the arrow is at position
        if (arrow.transform.localPosition.y == -0.875f)
        {
            canAdjustVolume = true;
        }
        else
        {
            canAdjustVolume = false;
        }
    }

    // Check if the arrow is at a specific position
    private void CheckForFullscreenToggle()
    {
        // If the arrow is at position
        if (arrow.transform.localPosition.y == -5f)
        {
            gameWindowMode = true;

            // Trigger fullscreen toggle if Enter is being held
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isHoldingEnter = true;  // Start holding Enter
            }
        }
        else
        {
            gameWindowMode = false;
        }
    }

    // Toggle fullscreen mode
    private void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            // If currently in fullscreen mode, switch to windowed mode and restore window size
            Screen.SetResolution(windowedWidth, windowedHeight, false);  // Set the stored windowed resolution
            fullScreen.SetActive(false);
            windowed.SetActive(true);
        }
        else
        {
            // Store current resolution before switching to fullscreen
            windowedWidth = Screen.width;
            windowedHeight = Screen.height;

            // Switch to fullscreen mode at native resolution
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, true);
            fullScreen.SetActive(true);
            windowed.SetActive(false);
        }

        // Optionally, you can add a log message to verify it's working
        Debug.Log("Fullscreen mode is now: " + (Screen.fullScreen ? "Enabled" : "Disabled"));
    }

    // Check if the arrow is at a specific position
    private void CheckForResetScore()
    {
        // If the arrow is at position
        if (arrow.transform.localPosition.y == -8.85f)
        {
            resetScoreMode = true;

            // Trigger fullscreen toggle if Enter is being held
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isHoldingEnter = true;  // Start holding Enter
            }
        }
        else
        {
            resetScoreMode = false;
        }
    }

    // Toggle reset score
    private void ToggleResetScore()
    {
        // Toggle the reset score
        PlayerPrefs.DeleteKey("HighScore");
        scoreManager.ResetScore();
        scoreManager.highScore = 0;
        Debug.Log("Score has been reset");
    }
}
