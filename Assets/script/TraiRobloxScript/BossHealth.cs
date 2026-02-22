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
    public float warningFadeSpeed = 2f; // <--- BIẾN MỚI: CHỈNH TỐC ĐỘ NHÁY Ở ĐÂY
    public AudioSource audioSource;
    public AudioClip warningSound;

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

        if (audioSource != null && warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }

        StartCoroutine(FadeWarningRoutine()); // Gọi hàm Fade mới thay cho Blink

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

    // --- HÀM MỚI: LÀM MỜ VÀ ĐẬM DẦN ---
    IEnumerator FadeWarningRoutine()
    {
        if (warningOverlay == null) yield break;

        warningOverlay.SetActive(true);

        // Tự động lấy CanvasGroup (hoặc thêm vào nếu chưa có) để làm mờ cả cụm UI một cách mượt nhất
        CanvasGroup canvasGroup = warningOverlay.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = warningOverlay.AddComponent<CanvasGroup>();
        }

        float endTime = Time.time + fillDuration;

        while (Time.time < endTime)
        {
            // Mathf.PingPong sẽ làm giá trị Alpha chạy trơn tru từ 0 đến 1 rồi vòng ngược lại từ 1 về 0
            canvasGroup.alpha = Mathf.PingPong(Time.time * warningFadeSpeed, 1f);

            yield return null; // Chạy mỗi frame để hiệu ứng siêu mượt
        }

        // Chạy xong intro thì tắt đi và reset độ mờ lại như cũ để đề phòng
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

        if (GameManager.instance != null)
        {
            GameManager.instance.WinGame();
        }

        gameObject.SetActive(false);
    }
}