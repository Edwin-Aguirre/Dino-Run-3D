using UnityEngine;
using UnityEngine.SceneManagement;

public class DinoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject title;

    [SerializeField]
    private GameObject gameOver;

    [SerializeField]
    private GameObject button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        button.SetActive(false);
    }

    public void ShowUI()
    {
        gameOver.SetActive(true);
        button.SetActive(true);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
