using UnityEngine;

// Kế thừa để giữ nguyên logic nhặt đồ của bạn bạn
public class WeaponItem : InteractableObject
{
    public override void PerformAction()
    {
        // Gọi logic gốc: phát âm thanh, xóa object dưới đất, thêm vào túi đồ...
        base.PerformAction();

        // Tìm Player và yêu cầu hiện khẩu súng đang ẩn trên tay
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerCombatLayerController controller = player.GetComponent<PlayerCombatLayerController>();
            if (controller != null)
            {
                controller.UnlockWeapon();
            }
        }
    }
}