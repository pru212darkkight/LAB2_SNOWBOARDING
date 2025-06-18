using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        Debug.Log($"Current Scene: {currentScene}");

        float highScore = GlobalScoreManager.Instance.GetScore(currentScene);
        float currentScore = ScoreManager.Instance.GetScore();

        Debug.Log($"High Score for {currentScene}: {highScore}");
        Debug.Log($"Current Score: {currentScore}");

        scoreText.text = $"High Score: {Mathf.FloorToInt(highScore)}\nCurrent Score: {Mathf.FloorToInt(currentScore)}";
    }
}
