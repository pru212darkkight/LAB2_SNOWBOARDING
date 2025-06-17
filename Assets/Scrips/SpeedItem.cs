using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 60f;  
    [SerializeField] private float boostDuration = 4f;      

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.BoostSpeedTemporarily(speedMultiplier, boostDuration);
                Destroy(gameObject); 
            }
        }
    }
}
