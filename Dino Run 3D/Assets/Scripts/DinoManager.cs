using UnityEngine;
using UnityEngine.UI;

public class DinoManager : MonoBehaviour
{
    private ScoreManager scoreManager;  // Manages the player's score
    private DinoDie dinoDie;  // Handles the Dino's death logic
    private DinoAnimations dinoAnimations;  // Controls animations for the Dino

    // Serialized fields for various UI elements that can be configured in the Inspector
    [SerializeField]
    private GameObject title;  // Title screen UI
    [SerializeField]
    private GameObject pause;  // Pause menu UI
    [SerializeField]
    private GameObject arrow;  // Arrow that moves in the menu
    [SerializeField]
    private GameObject fullScreen;  // UI element for fullscreen mode
    [SerializeField]
    private GameObject windowed;  // UI element for windowed mode
    [SerializeField]
    private GameObject resetScore;  // UI element for score reset option
    [SerializeField]
    private Image resetScoreImage;  // Image used to visualize reset score progress
    [SerializeField]
    private GameObject hold;  // UI element showing "hold" message
    [SerializeField]
    private GameObject[] hideObjects;  // Objects to hide when the game starts

    // Colliders for different menu sections that the arrow can snap to
    public BoxCollider[] sectionColliders;

    // Predefined Y positions the arrow can snap to in the menu
    public float[] snapPositions = { -0.875f, -5f, 5f };
    private int currentSnapIndex = 0;  // Index to track the current snap position

    // Flags and variables related to the pause and input management
    private bool isMoving = false;  // Prevents multiple movement inputs at once
    public bool isPaused = false;  // Checks if the game is paused
    public bool canAdjustVolume = false;  // Indicates if volume can be adjusted

    // Timer variables to track how long the Enter key is held down for
    private float enterHoldTime = 0f;  // Time the Enter key has been held
    private bool isHoldingEnter = false;  // Flag to track Enter key being held
    private bool gameWindowMode = false;  // Flag for fullscreen/windowed mode toggle
    private bool resetScoreMode = false;  // Flag for score reset mode

    // Store windowed resolution for later use
    private int windowedWidth;
    private int windowedHeight;

    void Start()
    {
        // Initialize references to other game systems
        scoreManager = FindAnyObjectByType<ScoreManager>();
        dinoDie = FindAnyObjectByType<DinoDie>();
        dinoAnimations = FindAnyObjectByType<DinoAnimations>();

        // Set initial game UI states
        title.SetActive(true);
        pause.SetActive(false);
        isPaused = false;

        // Hide game objects at the start
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

    void Update()
    {
        // Start the game when space or left-click is pressed, if the game isn't paused
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!scoreManager.hasStarted && !isPaused)
            {
                scoreManager.StartScore();
                scoreManager.hasStarted = true;
                title.SetActive(false);

                // Show game objects once the game starts
                foreach (var objects in hideObjects)
                {
                    objects.SetActive(true);
                }
            }
        }

        // Pause or resume the game when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();  // Resume the game if it's paused
                if (!scoreManager.hasStarted)
                {
                    title.SetActive(true);  // Show title screen if game hasn't started
                }
            }
            else
            {
                if (!dinoDie.hasDied)
                {
                    Pause();  // Pause the game if the Dino hasn't died
                }
            }
        }

        // Handle input and actions when the game is paused
        if (isPaused)
        {
            MenuIndicator();  // Move the arrow and handle UI navigation
            CheckForVolumeAdjustment();  // Check if the player is on the volume section
            CheckForFullscreenToggle();  // Check if the player is on the fullscreen section
            CheckForResetScore();  // Check if the player is on the reset score section
        }

        // Handle holding Enter for 3 seconds to trigger fullscreen or reset score
        if (isHoldingEnter)
        {
            enterHoldTime += Time.unscaledDeltaTime;  // Track how long Enter is held
            if (enterHoldTime >= 3f && gameWindowMode)
            {
                ToggleFullscreen();  // Toggle fullscreen after 3 seconds
                isHoldingEnter = false;  // Reset flag
            }
            if (enterHoldTime >= 3f && resetScoreMode)
            {
                ToggleResetScore();  // Reset score after 3 seconds
                isHoldingEnter = false;  // Reset flag
            }

            // Update reset score animation based on the time the Enter key has been held
            resetScoreImage.fillAmount = Mathf.Clamp01(enterHoldTime / 3f);
            dinoAnimations.PlayResetScoreShakeAnimation();
        }

        // Reset Enter key holding state when the key is released
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0))
        {
            isHoldingEnter = false;
            enterHoldTime = 0f;

            // Reset the fill amount of the reset score image when the key is released
            resetScoreImage.fillAmount = 0f;
            dinoAnimations.PlayResetScoreAnimation();
        }

        // Single press of Enter to toggle fullscreen
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (gameWindowMode)  // Only toggle fullscreen if the arrow is at the correct position
            {
                ToggleFullscreen();
            }
        }
    }

    // Pause the game, show the pause menu, and stop time
    void Pause()
    {
        pause.SetActive(true);
        title.SetActive(false);
        fullScreen.SetActive(!Screen.fullScreen);
        windowed.SetActive(Screen.fullScreen);
        resetScore.SetActive(true);
        Time.timeScale = 0f;  // Pause the game by setting time scale to 0
        isPaused = true;
    }

    // Resume the game, hide the pause menu, and restart time
    void Resume()
    {
        pause.SetActive(false);
        arrow.SetActive(false);
        fullScreen.SetActive(false);
        windowed.SetActive(false);
        resetScore.SetActive(false);
        Time.timeScale = 1f;  // Resume normal time flow
        isPaused = false;

        // Reset the position of the arrow when resuming
        arrow.transform.localPosition = new Vector3(4, arrow.transform.localPosition.y, arrow.transform.localPosition.z);
    }

    // Handle the movement of the arrow in the menu
    void MenuIndicator()
    {
        // Show the arrow when the menu is active
        arrow.SetActive(true);

        if (isMoving) return;  // Prevent input if the arrow is already moving

        // Raycast to detect mouse hover over a section and snap the arrow to the section
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Snap the arrow to the correct position based on which section the mouse is hovering over
            for (int i = 0; i < sectionColliders.Length; i++)
            {
                if (hit.collider == sectionColliders[i])
                {
                    currentSnapIndex = i;
                    StartCoroutine(SnapToPosition(snapPositions[currentSnapIndex]));  // Snap to the new position
                    break;
                }
            }
        }

        // Arrow movement with keyboard input (Up/Down arrow keys)
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Move the arrow down if not already at the bottom position
            if (currentSnapIndex < snapPositions.Length - 1)
            {
                currentSnapIndex++;
                StartCoroutine(SnapToPosition(snapPositions[currentSnapIndex]));
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Move the arrow up if not already at the top position
            if (currentSnapIndex > 0)
            {
                currentSnapIndex--;
                StartCoroutine(SnapToPosition(snapPositions[currentSnapIndex]));
            }
        }
    }

    // Coroutine to smoothly snap the arrow to the target position
    private System.Collections.IEnumerator SnapToPosition(float targetY)
    {
        isMoving = true;  // Prevent further movement while animating

        float startY = arrow.transform.localPosition.y;
        float elapsedTime = 0f;
        float duration = 0.2f;  // Duration of the snap animation

        // Animate the arrow's movement over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newY = Mathf.Lerp(startY, targetY, elapsedTime / duration);
            arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, newY, arrow.transform.localPosition.z);
            yield return null;  // Wait until the next frame
        }

        // Ensure the arrow's final position is set precisely
        arrow.transform.localPosition = new Vector3(arrow.transform.localPosition.x, targetY, arrow.transform.localPosition.z);
        isMoving = false;  // Allow movement again
    }

    // Check if the player is on the volume adjustment section
    private void CheckForVolumeAdjustment()
    {
        if (arrow.transform.localPosition.y == -0.875f)
        {
            canAdjustVolume = true;
            dinoAnimations.PlayVolumeAnimation();
        }
        else
        {
            canAdjustVolume = false;
            dinoAnimations.StopVolumeAnimation();
        }
    }

    // Check if the player is on the fullscreen toggle section
    private void CheckForFullscreenToggle()
    {
        if (arrow.transform.localPosition.y == -5f)
        {
            gameWindowMode = true;
            dinoAnimations.PlayFullscreenAnimation();
            dinoAnimations.PlayWindowedAnimation();
        }
        else
        {
            gameWindowMode = false;
            dinoAnimations.StopFullscreenAnimation();
            dinoAnimations.StopWindowedAnimation();
        }
    }

    // Toggle fullscreen mode on or off
    private void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(windowedWidth, windowedHeight, false);  // Switch to windowed mode
            fullScreen.SetActive(true);
            windowed.SetActive(false);
        }
        else
        {
            windowedWidth = Screen.width;
            windowedHeight = Screen.height;
            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, true);  // Switch to fullscreen mode
            fullScreen.SetActive(false);
            windowed.SetActive(true);
        }
    }

    // Check if the player is on the reset score section
    private void CheckForResetScore()
    {
        if (arrow.transform.localPosition.y == -8.85f)
        {
            resetScoreMode = true;
            hold.SetActive(true);

            if (!isHoldingEnter)
            {
                dinoAnimations.PlayResetScoreAnimation();
            }

            // Begin holding Enter to reset score
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                isHoldingEnter = true;
            }
        }
        else
        {
            resetScoreMode = false;
            hold.SetActive(false);

            // Stop reset score animation if not holding Enter
            if (!isHoldingEnter)
            {
                dinoAnimations.StopResetScoreAnimation();
            }
        }
    }

    // Reset the player's score
    private void ToggleResetScore()
    {
        PlayerPrefs.DeleteKey("HighScore");  // Delete the saved high score
        scoreManager.ResetScore();  // Reset the current score
        scoreManager.highScore = 0;  // Set high score to 0
    }
}
