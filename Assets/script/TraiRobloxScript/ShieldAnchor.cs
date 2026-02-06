using UnityEngine;

public class ShieldAnchor : MonoBehaviour
{
    private BossShieldSkill bossShieldManager;

    [Header("--- Âm thanh khi bị phá ---")]
    public AudioSource anchorAudioSource; // Gắn AudioSource của chính cái trụ (nếu có) hoặc tạo cái mới
    public AudioClip breakSound;          // Kéo tiếng kim loại vỡ/nổ vào đây

    public void Setup(BossShieldSkill manager)
    {
        bossShieldManager = manager;
    }

    public void TakeDamage()
    {
        // 1. Phát âm thanh bị phá vỡ (Quan trọng: Dùng PlayClipAtPoint)
        // Tại sao dùng PlayClipAtPoint? 
        // Vì dòng dưới bạn dùng SetActive(false), object này sẽ bị tắt ngay lập tức -> AudioSource trên nó cũng tắt theo -> Không nghe được tiếng nổ.
        // PlayClipAtPoint tạo ra một object tạm thời để phát xong âm thanh rồi tự hủy, dù cái trụ đã tắt.
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // 2. Tắt hiển thị của cục này
        gameObject.SetActive(false);

        // 3. Báo cho Boss biết
        if (bossShieldManager != null)
        {
            bossShieldManager.OnAnchorDestroyed();
        }
    }
}