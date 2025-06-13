using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsCloseButton;

    private bool isPaused = false;
    private bool isGameOver = false;
    private GameObject previousPanel; // Track which panel opened settings
    private Vector2 savedVelocity;
    private float savedAngularVelocity;

    private void Awake()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;

        // Make sure all panels are hidden initially
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ResumeGame);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToMenu);
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(RestartGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => OpenSettings(pausePanel));
        if (settingsCloseButton != null)
            settingsCloseButton.onClick.AddListener(CloseSettings);
    }

    private void Update()
    {
        // Check for pause input only if game is not over
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Don't allow pausing if game is over
        if (isGameOver) return;

        isPaused = !isPaused;

        // Toggle pause panel visibility
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // If unpausing, make sure settings panel is closed
        if (!isPaused && settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Store or restore velocity based on pause state
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (isPaused)
                {
                    // Store current velocities when pausing
                    savedVelocity = rb.linearVelocity;
                    savedAngularVelocity = rb.angularVelocity;
                }
            }
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
        // Hide all panels
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Find and reset player's physics
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Restore the exact velocities from before pausing
                rb.linearVelocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;
            }
        }

        // Unpause the game
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;

        // Clear the singleton instance
        Instance = null;

        // Load the menu scene
        SceneManager.LoadScene("GameMenu");

        // Destroy this GameManager object
        Destroy(gameObject);
    }

    public void RestartGame()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        // Store the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Clear the singleton instance
        Instance = null;

        // Load the scene
        SceneManager.LoadScene(currentSceneName);

        // Destroy this GameManager object
        Destroy(gameObject);
    }

    public void OpenSettings(GameObject callingPanel)
    {
        if (settingsPanel != null)
        {
            previousPanel = callingPanel; // Store which panel called settings
            callingPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            if (previousPanel != null)
            {
                previousPanel.SetActive(true);
            }
        }
    }

    // Method to open game over panel with delay
    public void ShowGameOverWithDelay(float delay)
    {
        Debug.Log($"ShowGameOverWithDelay called with delay: {delay}");
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOver Panel is not assigned in GameManager!");
            return;
        }
        StartCoroutine(ShowGameOverCoroutine(delay));
        Debug.Log("Started ShowGameOverCoroutine");
    }

    private IEnumerator ShowGameOverCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log("ShowGameOver called");
        if (gameOverPanel != null)
        {
            isGameOver = true;
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("GameOver Panel activated");
        }
    }


   
}