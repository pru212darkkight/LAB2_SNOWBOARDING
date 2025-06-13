using UnityEngine;

public class DustTrail : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private float minSpeed = 1f;
    private bool isOnGround = false;
    private Rigidbody2D rb;
    private PlayerController playerController;

    private void Start()
    {
        if (snowEffect == null)
        {
            snowEffect = GetComponent<ParticleSystem>();
            if (snowEffect == null)
            {
                Debug.LogError("No ParticleSystem found on " + gameObject.name);
                return;
            }
        }

        rb = GetComponentInParent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found in parent of " + gameObject.name);
        }

        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("No PlayerController found in parent of " + gameObject.name);
        }
    }

    private void Update()
    {
        if (snowEffect == null || rb == null || playerController == null) return;
        isOnGround = playerController.IsGrounded();
        if (!isOnGround)
        {
            if (snowEffect.isPlaying)
            {
                snowEffect.Stop();
            }
            return;
        }

        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (currentSpeed > minSpeed)
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