using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private string currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        UpdateScoreDisplay();
    }

    private void Update()
    {
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        float highScore = GlobalScoreManager.Instance.GetScore(currentScene);
        float currentScore = ScoreManager.Instance.GetScore();

#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"[WebGL Score] Scene: {currentScene}, High: {highScore}, Current: {currentScore}");
#endif

        scoreText.text = $"High Score: {Mathf.FloorToInt(highScore)}\nCurrent Score: {Mathf.FloorToInt(currentScore)}";
    }
}
