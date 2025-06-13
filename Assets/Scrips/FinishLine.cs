using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float loadDelay = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu gameObject cha có tag Player
        if (collision.transform.root.CompareTag("Player"))
        {
            Debug.Log("finish map");
            Invoke("ReloadScene", loadDelay);
            //GlobalScoreManager.Instance.SaveScore(playerName, ScoreManager.Instance.GetScore());

        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
