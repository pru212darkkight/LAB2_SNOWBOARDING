using UnityEngine;

public class DustTrail : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private float minSpeed = 1f;
    
    private void Start()
    {
        if (snowEffect == null)
        {
            snowEffect = GetComponent<ParticleSystem>();
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.magnitude > minSpeed)
            {
                snowEffect.Play();
            }
            else
            {
                snowEffect.Stop();
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            snowEffect.Stop();
        }
    }
} 