using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float loadDelay = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu gameObject cha có tag Player
        if (collision.transform.root.CompareTag("Player"))
        {
            Debug.Log("finish map");
            GameManager.Instance.ShowWinPanelWithDelay(loadDelay);
        }
    }
}
