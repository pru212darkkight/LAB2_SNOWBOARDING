using UnityEngine;

public class FallZone : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player rơi xuống vực - Game Over!");
            AudioManager.Instance.PlayMusic(AudioManager.Instance.loseMusic);
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null!");
                return;
            }

            // Lưu điểm số trước khi game over
            if (GlobalScoreManager.Instance != null && ScoreManager.Instance != null)
            {
                float currentScore = ScoreManager.Instance.GetScore();
                Debug.Log($"Saving score: {currentScore}");
                GlobalScoreManager.Instance.SaveScore(currentScore);
            }
            else
            {
                Debug.LogError("ScoreManager hoặc GlobalScoreManager is null!");
            }
            GameManager.Instance.ShowGameOverWithDelay(loadDelay);
        }
    }
}