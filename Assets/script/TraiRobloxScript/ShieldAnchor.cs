using UnityEngine;

// Kế thừa IDamageable để viên đạn (bulletProjectTile) có thể gọi hàm TakeDamage
public class ShieldAnchor : MonoBehaviour, IDamageable
{
    [Header("--- Cài đặt Máu (Neo) ---")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("--- Âm thanh khi bị phá ---")]
    public AudioSource anchorAudioSource;
    // Thay đổi: Dùng mảng AudioClip để chứa nhiều âm thanh khác nhau
    public AudioClip[] breakSounds;

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
        // Tạm thời comment dòng log máu này lại cho đỡ rối Console nhé
        // Debug.Log($"[ShieldAnchor] Neo bị bắn! Máu còn: {currentHealth}/{maxHealth}");

        // Khi máu <= 0 thì phát nổ
        if (currentHealth <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        isDestroyed = true;
        Debug.Log("==== [ShieldAnchor] BẮT ĐẦU NỔ NE0 ====");

        // 1. Phát âm thanh nổ ngẫu nhiên
        if (breakSounds != null && breakSounds.Length > 0)
        {
            Debug.Log($"[ShieldAnchor] Mảng âm thanh có {breakSounds.Length} file. Đang random...");

            // Chọn ngẫu nhiên một con số từ 0 đến (số lượng âm thanh - 1)
            int randomIndex = Random.Range(0, breakSounds.Length);
            AudioClip soundToPlay = breakSounds[randomIndex];

            if (soundToPlay != null)
            {
                Debug.Log($"[ShieldAnchor] THÀNH CÔNG: Đã chọn âm thanh vị trí [{randomIndex}] mang tên '{soundToPlay.name}'. Chuẩn bị phát!");
                // Đặt mức âm lượng là 1f (100%) và phát ngay tại Camera
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

        Debug.Log("==== [ShieldAnchor] KẾT THÚC NỔ NE0 ====");
    }
}