using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform playerTransform;

    private float score = 0f;
    private float lastXPos = 0f;

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

        lastXPos = playerTransform.position.x;
        UpdateScoreText();
    }

    void Update()
    {
        TrackMovementAndUpdateScore();
    }

    void TrackMovementAndUpdateScore()
    {
        float currentX = playerTransform.position.x;
        float distanceMoved = currentX - lastXPos;

        // Chỉ cộng điểm khi di chuyển về phía trước
        if (distanceMoved > 0)
        {
            score += distanceMoved;
            UpdateScoreText();
        }

        lastXPos = currentX;
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
        lastXPos = playerTransform.position.x;
        UpdateScoreText();
    }

    public float GetScore()
    {
        return score;
    }
}