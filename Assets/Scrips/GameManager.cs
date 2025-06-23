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
    [SerializeField] private GameObject winPanel;

    [Header("Win Star UI")]
    [SerializeField] private Image winStarImage;
    [SerializeField] private Sprite star0Sprite;
    [SerializeField] private Sprite star1Sprite;
    [SerializeField] private Sprite star2Sprite;
    [SerializeField] private Sprite star3Sprite;

    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isLevelComplete = false;
    private GameObject previousPanel; // Track which panel opened settings
    private Vector2 savedVelocity;
    private float savedAngularVelocity;

    private void Awake()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;
        isLevelComplete = false;
        // Make sure all panels are hidden initially
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

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
        AudioManager.Instance.PlayRandomMusic(AudioManager.Instance.gameMusic);
    }

    private void Update()
    {
        // Check for pause input only if game is not over
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver)
        {
            TogglePause();
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUIReferences();
    }
    private void RefreshUIReferences()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            pausePanel = canvas.transform.Find("PausePanel")?.gameObject;
            settingsPanel = canvas.transform.Find("SettingPanel")?.gameObject;
            gameOverPanel = canvas.transform.Find("GameOverPanel")?.gameObject;
            winPanel = canvas.transform.Find("WinPanel")?.gameObject;
            winStarImage = canvas.transform.Find("Stars")?.GetComponent<Image>();
        }
        if (pausePanel != null)
        {
            var resumeBtn = pausePanel.transform.Find("CloseButton")?.GetComponent<Button>();
            if (resumeBtn != null)
            {
                resumeBtn.onClick.RemoveAllListeners();
                resumeBtn.onClick.AddListener(ResumeGame);
            }
            var exitBtn = pausePanel.transform.Find("ExitButton")?.GetComponent<Button>();
            if (exitBtn != null)
            {
                exitBtn.onClick.RemoveAllListeners();
                exitBtn.onClick.AddListener(ExitToMenu);
            }
            var settingBtn = pausePanel.transform.Find("SettingButton")?.GetComponent<Button>();
            if (settingBtn != null)
            {
                settingBtn.onClick.RemoveAllListeners();
                settingBtn.onClick.AddListener(() => OpenSettings(pausePanel));
            }
            var playAgainBtn = pausePanel.transform.Find("RestartGameButton")?.GetComponent<Button>();
            if (playAgainBtn != null)
            {
                playAgainBtn.onClick.RemoveAllListeners();
                playAgainBtn.onClick.AddListener(RestartGame);
            }
        }
        if (settingsPanel != null)
        {
            var closeBtn = settingsPanel.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(CloseSettings);
            }
        }
        if (gameOverPanel != null)
        {
            var restartBtn = gameOverPanel.transform.Find("RestartGameButton")?.GetComponent<Button>();
            if (restartBtn != null)
            {
                restartBtn.onClick.RemoveAllListeners();
                restartBtn.onClick.AddListener(RestartGame);
            }
            var exitBtn = gameOverPanel.transform.Find("ExitButton")?.GetComponent<Button>();
            if (exitBtn != null)
            {
                exitBtn.onClick.RemoveAllListeners();
                exitBtn.onClick.AddListener(ExitToMenu);
            }
            var settingBtn = gameOverPanel.transform.Find("SettingButton")?.GetComponent<Button>();
            if (settingBtn != null)
            {
                settingBtn.onClick.RemoveAllListeners();
                settingBtn.onClick.AddListener(() => OpenSettings(gameOverPanel));
            }
        }
        if (winPanel != null)
        {
            var nextBtn = winPanel.transform.Find("NextLevelButton")?.GetComponent<Button>();
            if (nextBtn != null)
            {
                nextBtn.onClick.RemoveAllListeners();
                nextBtn.onClick.AddListener(NextLevel);
            }
            var exitBtn = winPanel.transform.Find("ExitButton")?.GetComponent<Button>();
            if (exitBtn != null)
            {
                exitBtn.onClick.RemoveAllListeners();
                exitBtn.onClick.AddListener(ExitToMenu);
            }
        }
    }

    public void TogglePause()
    {
        // Don't allow pausing if game is over
        if (isGameOver || isLevelComplete) return;

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
                    // Thông báo cho player về việc pause
                    player.OnGamePaused();
                }
                else
                {
                    // Thông báo cho player về việc resume
                    player.OnGameResumed();
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

        // Unpause the game first
        isPaused = false;
        Time.timeScale = 1f;

        // Find and reset player's physics
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            // Thông báo cho player về việc resume
            player.OnGameResumed();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Restore the exact velocities from before pausing
                rb.linearVelocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;
            }
        }
    }

    public void ExitToMenu()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;
        isLevelComplete = false;
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
        ScoreManager.isGameOver = false; // Reset biến isGameOver để điểm có thể tăng lại

        // Store the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Clear the singleton instance
        Instance = null;

        // Reset spin state nếu player còn tồn tại trước khi load lại scene
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.ResetSpinState();
        }

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
        //Debug.Log($"ShowGameOverWithDelay called with delay: {delay}");
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOver Panel is not assigned in GameManager!");
            return;
        }
        StartCoroutine(ShowGameOverCoroutine(delay));
        //Debug.Log("Started ShowGameOverCoroutine");
    }

    private IEnumerator ShowGameOverCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        //Debug.Log("ShowGameOver called");
        if (gameOverPanel != null)
        {
            isGameOver = true;
            ScoreManager.isGameOver = true; // Ngăn không cho tăng điểm nữa

            // Đợi thêm một chút để player dừng hẳn
            yield return new WaitForSecondsRealtime(0.5f);

            // Lưu điểm sau khi đã dừng hẳn
            if (GlobalScoreManager.Instance != null && ScoreManager.Instance != null)
            {
                float finalScore = ScoreManager.Instance.GetScore();
                Debug.Log($"Saving final score: {finalScore}");
                GlobalScoreManager.Instance.SaveScore(finalScore);
            }

            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            //Debug.Log("GameOver Panel activated");
        }

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.OnGameOver();
        }
    }
    public void ShowWinPanelWithDelay(float delay)
    {
        if (winPanel == null)
        {
            Debug.LogError("Win Panel is not assigned in GameManager!");
            return;
        }
        if (!isLevelComplete) // Only show if level is not already complete
        {
            StartCoroutine(ShowWinPanelCoroutine(delay));
        }
    }

    private IEnumerator ShowWinPanelCoroutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (winPanel != null)
        {
            isLevelComplete = true;
            ScoreManager.isGameOver = true; // Ngăn không cho tăng điểm nữa

            // Đợi thêm một chút để player dừng hẳn
            yield return new WaitForSecondsRealtime(0.5f);

            // Lưu điểm sau khi đã dừng hẳn
            if (GlobalScoreManager.Instance != null && ScoreManager.Instance != null)
            {
                float finalScore = ScoreManager.Instance.GetScore();
                Debug.Log($"[WIN] Saving final score: {finalScore}");
                GlobalScoreManager.Instance.SaveScore(finalScore);
            }

            // ✅ Lưu trạng thái hoàn thành level vào JSON
            GlobalScoreManager.Instance.MarkLevelCompleted();

            winPanel.SetActive(true);
            Time.timeScale = 0f;
            //Debug.Log("Level Complete! Win Panel activated");
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int stars = LevelCompletionChecker.GetStarCount(currentScene);
        if (winStarImage != null)
        {
            switch (stars)
            {
                case 1: winStarImage.sprite = star1Sprite; break;
                case 2: winStarImage.sprite = star2Sprite; break;
                case 3: winStarImage.sprite = star3Sprite; break;
                default: winStarImage.sprite = star0Sprite; break;
            }
        }

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.OnGameOverOrWin();
        }
    }


    public void NextLevel()
    {
        // Reset game state
        Time.timeScale = 1f;
        isPaused = false;
        isLevelComplete = false;
        ScoreManager.isGameOver = false; // Reset để điểm có thể tăng ở level mới

        // Load next scene
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            AudioManager.Instance.PlayMusic(AudioManager.Instance.upgradeLevel);
        }
        else
        {
            // If no more scenes, go back to menu
            ExitToMenu();
        }
    }

    public bool IsLevelComplete()
    {
        return isLevelComplete;
    }


}