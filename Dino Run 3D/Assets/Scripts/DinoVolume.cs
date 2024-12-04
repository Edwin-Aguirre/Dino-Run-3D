using UnityEngine;

public class DinoVolume : MonoBehaviour
{
    public GameObject volumeIndicatorPrefab; // The 3D model to use as a volume indicator
    public GameObject volumePrefab; // The 3D model of the volume name
    public AudioSource[] audioSources;
    public int maxVolume = 5; // Maximum volume level
    private int currentVolume = 0; // Current volume level
    private GameObject[] indicators; // Array to hold the indicators
    private GameObject lastUpdatedIndicator; // Track the last updated indicator
    private const string VolumeKey = "AudioVolume"; // Key for saving volume level

    public DinoManager dinoManager;

    void Start()
    {
        indicators = new GameObject[maxVolume];
        LoadVolume(); // Load the saved volume level
        UpdateVolumeUI(); // Show the initial volume level at the start
        UpdateAudioSourcesVolume(); // Update audio sources to match the initial volume

        HideUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && dinoManager.isPaused)
        {
            SetVolume(currentVolume + 1); // Increase volume
            PlayVolumeChangeSound();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && dinoManager.isPaused)
        {
            SetVolume(currentVolume - 1); // Decrease volume
            PlayVolumeChangeSound();
        }

        if (!dinoManager.isPaused)
        {
            HideUI();
        }
        if (dinoManager.isPaused)
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        volumePrefab.SetActive(true);

        foreach (var item in indicators)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }
    }

    private void HideUI()
    {
        volumePrefab.SetActive(false);

        foreach (var item in indicators)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }
    }

    // Call this method to change the volume level
    public void SetVolume(int volume)
    {
        currentVolume = Mathf.Clamp(volume, 0, maxVolume);
        UpdateVolumeUI();
        UpdateAudioSourcesVolume(); // Update audio sources based on the new volume level
        SaveVolume(); // Save the current volume level
    }

    // Update the UI to reflect the current volume level
    private void UpdateVolumeUI()
    {
        // Clear previous indicators
        foreach (var indicator in indicators)
        {
            if (indicator != null)
            {
                Destroy(indicator);
            }
        }

        // Create new indicators based on the current volume level
        for (int i = 0; i < currentVolume; i++)
        {
            indicators[i] = Instantiate(volumeIndicatorPrefab, transform);
            // Position the indicators
            indicators[i].transform.localPosition = new Vector3(13 + i * 2f, -0.875f, -5); // Adjust spacing as needed

            // If this is the last updated indicator, enable its Animator
            if (i == currentVolume - 1)
            {
                lastUpdatedIndicator = indicators[i];
                if (lastUpdatedIndicator.transform.childCount > 0)
                {
                    var animator = lastUpdatedIndicator.transform.GetChild(0).GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.enabled = true; // Enable Animator for the last indicator
                    }
                }
            }
        }
    }



    private void PlayVolumeChangeSound()
    {
        AudioSource audioSource = audioSources[audioSources.Length - 1];
        audioSource.Play();
    }

    // Update the volume of the audio sources based on the current volume level
    private void UpdateAudioSourcesVolume()
    {
        float volumeFactor = (float)currentVolume / maxVolume; // Calculate volume factor (0 to 1)
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volumeFactor; // Set audio source volume
            }
        }
    }

    // Save the current volume level
    private void SaveVolume()
    {
        PlayerPrefs.SetInt(VolumeKey, currentVolume);
        PlayerPrefs.Save(); // Ensure the data is saved
    }

    // Load the saved volume level
    public void LoadVolume()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            currentVolume = PlayerPrefs.GetInt(VolumeKey);
        }
        else
        {
            currentVolume = 5; // Default value if not set
        }
    }
}
