using UnityEngine;

public class WoodItem : MonoBehaviour
{

    private Animator animator;
    private bool hasTriggered = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // Ngăn animation chạy từ đầu
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Bật Animator và kích hoạt animation
            if (animator != null)
            {
                animator.enabled = true;
                animator.SetTrigger("Activate");
            }

            // Phát âm thanh khi va chạm
            AudioManager.Instance.PlaySFX(AudioManager.Instance.getWood);

            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.IsInvulnerable()) // Kiểm tra có khiên không
                {
                    player.ForceDeactivateShield(); // Huỷ khiên
                    return;                     // Không làm chậm nữa
                }
                else
                {
                    player.ReduceSpeedTemporarily(0.5f, 3f);
                }
            }
        }
    }

    // Hàm này sẽ được gọi từ Animation Event cuối clip
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
