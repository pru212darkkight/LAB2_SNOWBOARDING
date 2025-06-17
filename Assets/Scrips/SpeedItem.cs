using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    [SerializeField] private float speedBoostAmount = 20f;   // Lượng lực boost thêm
    [SerializeField] private float boostDuration = 6f;       // Tổng thời gian hiệu lực (3s tăng dần + 3s giữ nguyên)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.BoostSpeedTemporarily(speedBoostAmount, boostDuration);
            Destroy(gameObject); // Xoá item sau khi ăn
        }
    }
}
