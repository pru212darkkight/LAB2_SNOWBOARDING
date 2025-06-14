using UnityEngine;

public class IgnoreTriggerOnceIfShielded : MonoBehaviour
{
    private bool hasIgnoredPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerShield shield = other.GetComponent<PlayerShield>();
        if (shield != null && shield.IsShieldActive)
        {
            if (!hasIgnoredPlayer)
            {
                Debug.Log("Player có khiên - miễn nhiễm 1 lần với item: " + gameObject.name);
                hasIgnoredPlayer = true;        // Không xử lý va chạm lần này
                return;
            }
        }

        // Nếu không có khiên hoặc đã hết khiên thì xử lý bình thường
        HandleImpact(other.gameObject);
    }

    private void HandleImpact(GameObject player)
    {
        Debug.Log("Player bị ảnh hưởng bởi item: " + gameObject.name);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOverWithDelay(0.5f);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }
}
