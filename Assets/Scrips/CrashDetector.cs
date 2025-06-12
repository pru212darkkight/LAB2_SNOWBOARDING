using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered with: " + collision.name);
        if (collision.CompareTag("Ground"))
        {
            Debug.Log("game over");
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager.Instance is null!");
                return;
            }
            GameManager.Instance.ShowGameOverWithDelay(loadDelay);
            Debug.Log("Called ShowGameOverWithDelay");
        }
    }
}
