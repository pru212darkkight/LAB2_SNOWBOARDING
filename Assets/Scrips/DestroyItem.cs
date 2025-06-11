using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Đã va chạm với: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Va chạm với Player! Restarting...");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
