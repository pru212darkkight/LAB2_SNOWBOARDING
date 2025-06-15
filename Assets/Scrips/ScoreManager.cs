using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform playerTransform;

    private float score = 0f;
    private float maxXPos = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform chưa được gán trong ScoreManager!");
            return;
        }

        maxXPos = playerTransform.position.x;
        UpdateScoreText();
    }

    void Update()
    {
        UpdateScoreBasedOnFurthestPosition();
    }

    void UpdateScoreBasedOnFurthestPosition()
    {
        float currentX = playerTransform.position.x;

        if (currentX > maxXPos)
        {
            float distanceGained = currentX - maxXPos;
            score += distanceGained;
            maxXPos = currentX;
            UpdateScoreText();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }

        Debug.Log("Score: " + Mathf.FloorToInt(score));
    }

    public void ResetScore()
    {
        score = 0;
        maxXPos = playerTransform.position.x;
        UpdateScoreText();
    }

    public float GetScore()
    {
        return score;
    }
}
