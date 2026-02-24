using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("--- Chỉ số Boss ---")]
    public float maxHealth = 1000f;
    private float currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = true;

    [Header("--- UI Thanh Máu ---")]
    public GameObject bossUIPanel;
    public Slider healthSlider;
    public float fillDuration = 2.5f;

    [Header("--- Cảnh báo (Warning) ---")]
    public GameObject warningOverlay;
    [Tooltip("Tốc độ mờ/đậm của viền cảnh báo. Số càng to nháy càng nhanh.")]
    public float warningFadeSpeed = 2f;
    public AudioSource audioSource;
    public AudioClip warningSound;

    [Tooltip("THÊM MỚI: Thời gian phát âm thanh cảnh báo (giây) và hiệu ứng nháy đỏ.")]
    public float warningSoundDuration = 2.5f;

    [Header("--- Kết nối Skill Boss ---")]
    public BossLaserSkill bossSkill;
    public BossShieldSkill shieldSkill;

    void Awake()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(false);
        if (warningOverlay != null) warningOverlay.SetActive(false);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = 0;
        }
    }

    public void StartFighting()
    {
        StartCoroutine(IntroHealthBarRoutine());
    }

    IEnumerator IntroHealthBarRoutine()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(true);

        StartCoroutine(PlayAndStopWarningSound());
        StartCoroutine(FadeWarningRoutine());

        float timer = 0f;
        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fillDuration;
            healthSlider.value = Mathf.Lerp(0, maxHealth, progress);
            yield return null;
        }

        healthSlider.value = maxHealth;
        currentHealth = maxHealth;
        isInvulnerable = false;
    }

    IEnumerator PlayAndStopWarningSound()
    {
        if (audioSource != null && warningSound != null)
        {
            audioSource.clip = warningSound;
            audioSource.Play();
            yield return new WaitForSeconds(warningSoundDuration);
            audioSource.Stop();
        }
    }

    IEnumerator FadeWarningRoutine()
    {
        if (warningOverlay == null) yield break;

        warningOverlay.SetActive(true);

        CanvasGroup canvasGroup = warningOverlay.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = warningOverlay.AddComponent<CanvasGroup>();
        }

        // ĐÃ SỬA: Dùng warningSoundDuration để quyết định thời gian nháy UI thay cho fillDuration
        float endTime = Time.time + warningSoundDuration;

        while (Time.time < endTime)
        {
            canvasGroup.alpha = Mathf.PingPong(Time.time * warningFadeSpeed, 1f);
            yield return null;
        }

        warningOverlay.SetActive(false);
        canvasGroup.alpha = 1f;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;

        if (shieldSkill != null && !shieldSkill.IsSkillFinished())
        {
            Debug.Log("[Boss] Đang bật khiên! Miễn nhiễm sát thương.");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"[Boss] Bị bắn! Máu còn: {currentHealth}/{maxHealth}");

        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("BOSS ĐÃ BỊ TIÊU DIỆT!");

        if (bossUIPanel != null) bossUIPanel.SetActive(false);

        if (bossSkill != null) bossSkill.StopAllCoroutines();
        if (shieldSkill != null) shieldSkill.StopAllCoroutines();

        CheckPlayer checkPlayerScript = FindObjectOfType<CheckPlayer>();
        if (checkPlayerScript != null)
        {
            checkPlayerScript.StopBossMusic();
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.WinGame();
        }

        gameObject.SetActive(false);
    }
}