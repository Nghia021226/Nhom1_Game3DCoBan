using UnityEngine;

public class MeatItem : InteractableObject
{
    public override string GetHintText()
    {
        return "Giữ E để nhặt Thịt";
    }

    public override void PerformAction()
    {
        // 1. Kiểm tra tay bận
        if (GameManager.instance.hasMeat)
        {
            GameManager.instance.ShowHint("Tay đang bận!");
            return;
        }

        // 2. Cập nhật trạng thái logic
        GameManager.instance.hasMeat = true;
        GameManager.instance.ShowHint("Đã nhặt thịt! (Nhấn F để thả)");
        PlayInteractSound();

        // --- DÒNG MỚI THÊM: CẬP NHẬT UI ---
        // Gọi hàm này để icon bay vào ô slot đầu tiên
        // Lưu ý: Bro phải gắn ảnh vào biến itemIcon trong Inspector nhé
        GameManager.instance.AddItemToHotbar(InteractableObject.ItemType.Meat, itemIcon);
        // ----------------------------------

        // 4. Xóa vật phẩm trên đất
        Destroy(gameObject);
    }
}