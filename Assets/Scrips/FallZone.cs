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

            GameManager.Instance.ShowGameOverWithDelay(loadDelay);
        }
    }
}