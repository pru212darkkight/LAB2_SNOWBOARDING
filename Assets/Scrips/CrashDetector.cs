using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Debug.Log("game over");
            //GlobalScoreManager.Instance.SaveScore(playerName, ScoreManager.Instance.GetScore());

            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null!");
                return;
            }
            if (GlobalScoreManager.Instance == null)
            {
                Debug.LogError("GlobalScoreManager.Instance is null!");
            }
            else
            {
                float currentScore = ScoreManager.Instance.GetScore();
                Debug.Log($"Saving score: {currentScore}");
                GlobalScoreManager.Instance.SaveScore(currentScore);
            }
            GameManager.Instance.ShowGameOverWithDelay(loadDelay);
        }
    }
}
