using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI spinBonusText;
    private Coroutine spinBonusCoroutine;
    private int bonus = 100;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.getStars);
            ScoreManager.Instance.AddScore(bonus);
            ScoreManager.Instance.ShowSpinBonus(bonus);
            Destroy(gameObject); 
        }
    }
}
