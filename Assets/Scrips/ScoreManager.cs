using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public static bool isGameOver = false;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TextMeshProUGUI spinBonusText;
    private Coroutine spinBonusCoroutine;


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
        if (isGameOver) return;
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
    }

    public void ResetScore()
    {
        score = 0;
        maxXPos = playerTransform.position.x;
        isGameOver = false;
        UpdateScoreText();
    }

    public float GetScore()
    {
        return score;
    }

    public void AddScore(float amount)
    {
        score += amount;
        UpdateScoreText();
    }
    public void AddSpinScore(int spinCount)
    {
        if (spinCount <= 0) return;
        int bonus = 0;
        if (spinCount == 1)
            bonus = 100;
        else
            bonus = 100 + (spinCount - 1) * 150;

        AddScore(bonus);
        ShowSpinBonus(bonus);
        Debug.Log("Spin: " + spinCount + " | Bonus spin score: " + bonus);
    }
    public void ShowSpinBonus(int bonus)
    {
        if (spinBonusCoroutine != null)
            StopCoroutine(spinBonusCoroutine);

        spinBonusCoroutine = StartCoroutine(SpinBonusRoutine(bonus));
    }

    public IEnumerator SpinBonusRoutine(int bonus)
    {
        spinBonusText.text = "+" + bonus.ToString();
        spinBonusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        spinBonusText.gameObject.SetActive(false);
    }

}
