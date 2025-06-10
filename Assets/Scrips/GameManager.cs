using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Pause Settings")]
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Make sure pause panel is hidden at start
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        // Toggle pause panel visibility
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // Set time scale (0 for pause, 1 for normal)
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }
} 