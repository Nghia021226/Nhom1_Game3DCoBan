using UnityEngine;

public class MeatItem : InteractableObject
{
    public override string GetHintText()
    {
        return "Giữ E để nhặt xương";
    }

    public override void PerformAction()
    {
       
        if (GameManager.instance.hasMeat)
        {
            GameManager.instance.ShowHint("Tay đang bận!");
            return;
        }

        
        GameManager.instance.hasMeat = true;
        GameManager.instance.ShowHint("Đã nhặt xương! (Nhấn F để thả)");
        PlayInteractSound();

       
        GameManager.instance.AddItemToHotbar(InteractableObject.ItemType.Meat, itemIcon);
       
        Destroy(gameObject);
    }
}