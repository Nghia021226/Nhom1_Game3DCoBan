using UnityEngine;

public class AmmoLootItem : InteractableObject
{
    public int ammoAmount = 20;

    public override string GetHintText() => "Giữ E để nhặt đạn";

    private void Awake()
    {
        // QUAN TRỌNG: Đặt type là Item để script CameraZoom chịu tương tác
        type = ObjectType.Item;
        holdTime = 0.5f; // Thời gian giữ E để nhặt (nếu muốn)
    }
    public override void PerformAction()
    {
        // Tìm người chơi và cộng đạn
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var shooter = player.GetComponent<ThirdPersonShooterController>();
            if (shooter != null)
            {
                shooter.AddAmmo(ammoAmount);
                Destroy(gameObject); // Nhặt xong thì biến mất
            }
        }
    }
}