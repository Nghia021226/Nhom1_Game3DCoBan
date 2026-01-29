using UnityEngine;

// Kế thừa từ InteractableObject để sử dụng các thiết lập có sẵn
public class WeaponItem : InteractableObject
{
    // --- ĐOẠN CODE THÊM MỚI ---
    public override string GetHintText()
    {
        // Thay vì lấy tên từ Enum, mình trả về nội dung tùy ý cho Súng
        return "Giữ E để nhặt Súng";
    }
    public override void PerformAction()
    {
        // KHÔNG gọi base.PerformAction() vì nó sẽ tự thêm súng vào kho đồ

        // 1. Dọn dẹp các UI tương tác cũ (Loading bar, Hint)
        if (GameManager.instance != null)
        {
            GameManager.instance.StopLoading();
            GameManager.instance.HideHint();
        }

        // 2. PHÁT ÂM THANH TRỰC TIẾP TẠI ĐÂY
        if (interactSound != null)
        {
            Debug.Log("[WeaponItem] Dang phat am thanh: " + interactSound.name);

            // Dùng vị trí của Camera để âm thanh phát ngay bên tai người chơi, 
            // đảm bảo không bị nhỏ do khoảng cách.
            AudioSource.PlayClipAtPoint(interactSound, Camera.main.transform.position, 1f);
        }
        else
        {
            // Nếu bạn thấy dòng này hiện ra ở Console, nghĩa là bạn chưa kéo file nhạc vào ô Interact Sound
            Debug.LogError("[WeaponItem] LOI: Chua gan file am thanh vao o Interact Sound!");
        }

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