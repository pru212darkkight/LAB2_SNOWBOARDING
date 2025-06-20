using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.getStars);
            ScoreManager.Instance.AddScore(100f);
            Destroy(gameObject); 
        }
    }
}
