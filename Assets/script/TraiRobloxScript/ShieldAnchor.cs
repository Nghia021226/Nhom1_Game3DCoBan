using UnityEngine;

public class ShieldAnchor : MonoBehaviour
{
    private BossShieldSkill bossShieldManager;

    // Hàm này gọi khi khởi tạo để Anchor biết ai quản lý nó
    public void Setup(BossShieldSkill manager)
    {
        bossShieldManager = manager;
    }

    // Hàm này bạn sẽ gọi từ đạn của Player khi bắn trúng cái cục này
    public void TakeDamage()
    {
        // 1. Tắt hiển thị của cục này đi (như bạn yêu cầu: ẩn chứ ko destroy)
        gameObject.SetActive(false);

        // 2. Báo cho Boss biết là "Tui bị phá rồi"
        if (bossShieldManager != null)
        {
            bossShieldManager.OnAnchorDestroyed();
        }
    }
}