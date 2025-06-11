using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            animator.SetTrigger("Play");
            hasPlayed = true;
        }
    }

    // Gọi từ animation event để xóa object
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
