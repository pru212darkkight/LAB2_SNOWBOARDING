using UnityEngine;

public class HelmetItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                //âm thanh nhận khiên
                AudioManager.Instance.PlaySFX(AudioManager.Instance.getShields);
                controller.ActivateShield(5f);
            }

            Destroy(gameObject);
        }
    }
}
