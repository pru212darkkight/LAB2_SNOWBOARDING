using UnityEngine;

public class DustTrail : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private float minSpeed = 1f;
    private bool isOnGround = false;
    private Rigidbody2D rb;

    private void Start()
    {
        if (snowEffect == null)
        {
            snowEffect = GetComponent<ParticleSystem>();
        }
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isOnGround && rb != null)
        {
            if (Mathf.Abs(rb.linearVelocity.x) > minSpeed)
            {
                if (!snowEffect.isPlaying)
                {
                    snowEffect.Play();
                }
            }
            else
            {
                if (snowEffect.isPlaying)
                {
                    snowEffect.Stop();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
            snowEffect.Stop();
        }
    }
}