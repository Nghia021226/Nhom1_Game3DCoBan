using UnityEngine;


public class PlayerCombatLayerController : MonoBehaviour
{
    private Animator animator;
    private bool hasWeapon = false;
    private bool isArmed = false;

    [Header("Cài đặt phím")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("Cài đặt Hiển thị")]
    [SerializeField] private GameObject weaponInHand;

    [Header("Cài đặt Animator Layer")]
    [SerializeField] private string combatLayerName = "Aim Layer";

    
   

    private int combatLayerIndex;

    void Start()
    {
        animator = GetComponent<Animator>();
        combatLayerIndex = animator.GetLayerIndex(combatLayerName);
        
        if (weaponInHand != null) weaponInHand.SetActive(false);
        animator.SetLayerWeight(combatLayerIndex, 0f);
    }

    void Update()
    {
        if (hasWeapon && Input.GetKeyDown(toggleKey))
        {
            isArmed = !isArmed;
            UpdateVisuals();
        }
    }

    public bool GetIsArmed()
    {
        return isArmed;
    }

    public void UnlockWeapon()
    {
        hasWeapon = true;
        isArmed = true;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (weaponInHand != null) weaponInHand.SetActive(isArmed);
        animator.SetLayerWeight(combatLayerIndex, isArmed ? 1f : 0f);
    }
}