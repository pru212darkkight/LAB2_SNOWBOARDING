using UnityEngine;

public class StoneShieldInteraction : MonoBehaviour
{
    private Collider2D physicalCollider;

    private void Awake()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger)
            {
                physicalCollider = col;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.IsInvulnerable())
            {
                if (physicalCollider != null)
                {
                    physicalCollider.enabled = false;
                    Debug.Log("Đã vô hiệu hóa collider vật lý do player có khiên");
                }
            }
        }
    }

    public void ReactivateCollider()
    {
        if (physicalCollider != null && !physicalCollider.enabled)
        {
            physicalCollider.enabled = true;
            Debug.Log("Collider vật lý đã được bật lại");
        }
    }


}
