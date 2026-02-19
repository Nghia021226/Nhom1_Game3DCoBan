using UnityEngine;

// Kế thừa IDamageable để viên đạn (bulletProjectTile) có thể gọi hàm TakeDamage
public class ShieldAnchor : MonoBehaviour, IDamageable
{
    [Header("--- Cài đặt Máu (Neo) ---")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("--- Âm thanh khi bị phá ---")]
    public AudioSource anchorAudioSource;
    public AudioClip breakSound;

    [Header("--- Hiệu ứng Nổ (Tùy chọn) ---")]
    public GameObject explosionVFX; // Kéo prefab nổ vào đây nếu có, không có thì bỏ trống

    private BossShieldSkill bossShieldManager;
    private bool isDestroyed = false; // Cờ chặn lỗi đạn găm nhiều viên cùng lúc

    // Hàm này được Boss gọi mỗi khi Boss thi triển lại skill Khiên
    public void Setup(BossShieldSkill manager)
    {
        bossShieldManager = manager;

        // QUAN TRỌNG: Hồi đầy máu và reset lại trạng thái mỗi lần Neo được bật lên
        currentHealth = maxHealth;
        isDestroyed = false;
    }

    // Giao tiếp với đạn của Player thông qua IDamageable
    public void TakeDamage(float damage)
    {
        // Nếu Neo đã nổ rồi thì không nhận sát thương nữa
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log($"[ShieldAnchor] Neo bị bắn! Máu còn: {currentHealth}/{maxHealth}");

        // Khi máu <= 0 thì phát nổ
        if (currentHealth <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        isDestroyed = true;

        // 1. Phát âm thanh nổ
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // 2. Tạo hiệu ứng hạt (nếu có)
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        // 3. Ẩn cục Neo đi để tái sử dụng sau (KHÔNG Destroy)
        gameObject.SetActive(false);

        // 4. Báo cho Boss biết Neo này đã sập
        if (bossShieldManager != null)
        {
            bossShieldManager.OnAnchorDestroyed();
        }
    }
}