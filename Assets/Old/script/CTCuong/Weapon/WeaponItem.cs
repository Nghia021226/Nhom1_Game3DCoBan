using UnityEngine;

public class WeaponItem : InteractableObject
{
    public override string GetHintText()
    {
        return "Giữ E để nhặt Súng";
    }
    public override void PerformAction()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StopLoading();
            GameManager.instance.HideHint();
        }

        if (interactSound != null)
        {
            Debug.Log("[WeaponItem] Dang phat am thanh: " + interactSound.name);
            AudioSource.PlayClipAtPoint(interactSound, Camera.main.transform.position, 1f);
        }
        else
        {
            Debug.LogError("[WeaponItem] LOI: Chua gan file am thanh vao o Interact Sound!");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerCombatLayerController controller = player.GetComponent<PlayerCombatLayerController>();
            if (controller != null)
            {
                controller.UnlockWeapon(); 
            }
        }
        Destroy(gameObject);
    }
}