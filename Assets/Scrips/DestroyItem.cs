using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyItem : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Đã va chạm với: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Va chạm với Player! Restarting...");
            Debug.Log("game over");
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null!");
                return;
            }
            GameManager.Instance.ShowGameOverWithDelay(loadDelay);
        }
    }
}
