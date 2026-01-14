using UnityEngine;

// Kế thừa từ InteractableObject để sử dụng các thiết lập có sẵn
public class WeaponItem : InteractableObject
{
    public override void PerformAction()
    {
        // KHÔNG gọi base.PerformAction() vì nó sẽ tự thêm súng vào kho đồ

        // 1. Dọn dẹp các UI tương tác cũ (Loading bar, Hint)
        if (GameManager.instance != null)
        {
            GameManager.instance.StopLoading();
            GameManager.instance.HideHint();
        }

        // 2. Phát âm thanh nhặt súng (sử dụng hàm có sẵn từ lớp cha)
        PlayInteractSound();

        // 3. Tìm Player và kích hoạt khẩu súng đang ẩn trên tay
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerCombatLayerController controller = player.GetComponent<PlayerCombatLayerController>();
            if (controller != null)
            {
                controller.UnlockWeapon(); // Hàm này sẽ hiện súng trên tay và bật Combat Layer
            }
        }

        // 4. Xóa khẩu súng "vật lý" dưới sàn
        Destroy(gameObject);
    }
}