using UnityEngine;

public class AmmoLootItem : InteractableObject
{
    public int ammoAmount = 20;
    public override string GetHintText() => "Giữ E để nhặt đạn năng lượng";

    private void Awake()
    {
        type = ObjectType.Item;
        holdTime = 0.5f; 
    }
    public override void PerformAction()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var shooter = player.GetComponent<ThirdPersonShooterController>();
            if (shooter != null)
            {
                shooter.AddAmmo(ammoAmount);
                Destroy(gameObject); 
            }
        }
    }
}