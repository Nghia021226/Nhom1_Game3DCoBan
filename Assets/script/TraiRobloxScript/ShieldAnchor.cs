using UnityEngine;
public class ShieldAnchor : MonoBehaviour, IDamageable
{
    [Header("--- Cài đặt Máu (Neo) ---")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    [Header("--- Âm thanh khi bị phá ---")]
    [SerializeField] AudioSource anchorAudioSource;
    [SerializeField] AudioClip[] breakSounds;

    private BossShieldSkill bossShieldManager;
    private bool isDestroyed = false;
    public void Setup(BossShieldSkill manager)
    {
        bossShieldManager = manager;
        currentHealth = maxHealth;
        isDestroyed = false;
    }
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        Debug.Log($"[ShieldAnchor] Neo bị bắn! Máu còn: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Explode();
        }
    }
    private void Explode()
    {
        isDestroyed = true;
        Debug.Log("==== [ShieldAnchor] BẮT ĐẦU NỔ NE0 ====");
        if (breakSounds != null && breakSounds.Length > 0)
        {
            Debug.Log($"[ShieldAnchor] Mảng âm thanh có {breakSounds.Length} file. Đang random...");
            int randomIndex = Random.Range(0, breakSounds.Length);
            AudioClip soundToPlay = breakSounds[randomIndex];

            if (soundToPlay != null)
            {
                Debug.Log($"[ShieldAnchor] THÀNH CÔNG: Đã chọn âm thanh vị trí [{randomIndex}] mang tên '{soundToPlay.name}'. Chuẩn bị phát!");
                AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, 1f);
            }
            else
            {
                Debug.LogWarning($"[ShieldAnchor] LỖI: Vị trí [{randomIndex}] trong mảng bị trống (None). Bạn chưa kéo file âm thanh vào ô này!");
            }
        }
        else
        {
            Debug.LogWarning("[ShieldAnchor] LỖI: Mảng breakSounds đang trống (Size = 0) hoặc chưa được khởi tạo. Hãy kiểm tra lại Inspector!");
        }

        gameObject.SetActive(false);

        if (bossShieldManager != null)
        {
            bossShieldManager.OnAnchorDestroyed();
        }
        Debug.Log("==== [ShieldAnchor] KẾT THÚC NỔ NE0 ====");
    }
}