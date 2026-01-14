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

    [Header("Sửa lỗi hướng súng")]
    [SerializeField] private Transform chestBone; // Kéo Spine2 vào đây
    [SerializeField] private Vector3 rotationOffset; // Chỉnh X âm để súng chúc xuống

    [Header("Cài đặt Animator Layer")]
    [SerializeField] private string combatLayerName = "Combat Layer";

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
        // Nhấn Tab để rút/cất súng
        if (hasWeapon && Input.GetKeyDown(toggleKey))
        {
            isArmed = !isArmed;
            UpdateVisuals();
        }

        // Bắn bằng chuột trái
        if (hasWeapon && isArmed)
        {
            animator.SetBool("IsShooting", Input.GetMouseButton(0));
        }
    }

    // Chạy sau Animation để ép góc xoay xương ngực thẳng lại
    void LateUpdate()
    {
        if (hasWeapon && isArmed && chestBone != null)
        {
            chestBone.rotation *= Quaternion.Euler(rotationOffset);
        }
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