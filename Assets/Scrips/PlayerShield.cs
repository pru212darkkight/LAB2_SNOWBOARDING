using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private GameObject helmetShieldPrefab;
    private GameObject helmetShieldInstance;
    private Animator anim;

    public bool IsShieldActive { get; private set; } = false;

    public void ActivateShield(float duration)
    {
        
        if (helmetShieldInstance != null)
        {
            Destroy(helmetShieldInstance);
        }

        helmetShieldInstance = Instantiate(helmetShieldPrefab, transform);
        helmetShieldInstance.transform.localPosition = Vector3.zero;

        anim = helmetShieldInstance.GetComponentInChildren<Animator>();

        IsShieldActive = true; // Kích hoạt khiên
        AudioManager.Instance.PlaySFX(AudioManager.Instance.getShields);
        StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        if (anim == null)
        {
            Debug.LogWarning("Animator not found in HelmetShield prefab's children.");
            yield break;
        }

        anim.Play("ShieldAppear");

        yield return new WaitUntil(() =>
        {
            return anim != null &&
                   anim.GetCurrentAnimatorStateInfo(0).IsName("ShieldIdle");
        });

        float idleTime = duration - 2f;
        if (idleTime > 0)
        {
            yield return new WaitForSeconds(idleTime);
        }

        if (helmetShieldInstance != null && anim != null)
        {
            anim.SetTrigger("FadeOut");
            yield return new WaitForSeconds(2f);
        }

        if (helmetShieldInstance != null)
        {
            Destroy(helmetShieldInstance);
            helmetShieldInstance = null;
        }

        IsShieldActive = false; // Tắt khiên
        AudioManager.Instance.PlaySFX(AudioManager.Instance.optionButtonClickSound);
        anim = null;
    }

    public void ConsumeShield()
    {
        if (!IsShieldActive || helmetShieldInstance == null) return;

        StopAllCoroutines(); // Dừng shield routine

        if (anim != null)
        {
            anim.SetTrigger("FadeOut");
        }

        Destroy(helmetShieldInstance, 1f);

        helmetShieldInstance = null;
        IsShieldActive = false;
        anim = null;

        Debug.Log("Shield consumed after impact.");
    }
}
