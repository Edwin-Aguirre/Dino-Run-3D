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
    private GameObject[] hideObjects;

    public bool isPaused = false;

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
    }

    void Pause()
    {
        pause.SetActive(true);
        title.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
