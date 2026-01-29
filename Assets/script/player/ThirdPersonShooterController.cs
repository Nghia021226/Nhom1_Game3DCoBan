using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Camera & Sensitivity")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.5f;

    [Header("Shooting & VFX")]
    [SerializeField] private LayerMask aimCollierMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectTile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    // Thêm vào vùng Header Shooting & VFX trong file ThirdPersonShooterController.cs
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserShootSound;
    [Range(0f, 2f)][SerializeField] private float volume = 1f;

    [Header("Animation Rigging")]
    [SerializeField] private Rig rig1; // Tay phải
    [SerializeField] private Rig rig2; // Tay trái

    [Header("UI Settings")]
    [SerializeField] private GameObject crosshairUI; // Kéo cái hình tâm ngắm trên Canvas vào đây

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private PlayerCombatLayerController combatController;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        combatController = GetComponent<PlayerCombatLayerController>();
    }

    private void Update()
    {
        // 1. KIỂM TRA ĐIỀU KIỆN: Phải có súng và đang cầm trên tay mới được hành động
        bool hasWeaponArmed = combatController != null && combatController.GetIsArmed();

        // Chỉ coi là đang ngắm nếu người chơi nhấn chuột PHẢI và phải ĐANG CÓ SÚNG
        bool isCurrentlyAiming = starterAssetsInputs.aim && hasWeaponArmed;

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimCollierMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        // --- XỬ LÝ NGẮM BẮN (AIM) ---
        if (isCurrentlyAiming)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            if (crosshairUI != null) crosshairUI.SetActive(true); // HIỆN tâm ngắm khi nhắm

            // Bật Rigging và Animator Layer (Weight tiến về 1)
            float lerpSpeed = Time.deltaTime * 13f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, 1f, lerpSpeed);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, 1f, lerpSpeed);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, lerpSpeed));

            // Xoay nhân vật theo hướng chuột
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            // Trở về trạng thái bình thường (khi thả chuột hoặc không có súng)
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            if (crosshairUI != null) crosshairUI.SetActive(false); // ẨN tâm ngắm khi thôi nhắm

            // Tắt Rigging và Animator Layer (Weight tiến về 0)
            float lerpSpeed = Time.deltaTime * 13f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, 0f, lerpSpeed);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, 0f, lerpSpeed);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, lerpSpeed));
        }

        // --- XỬ LÝ BẮN (SHOOT) ---
        // Chỉ cho bắn nếu: Nhấn chuột trái + Đang ngắm + Đang có súng
        if (starterAssetsInputs.shoot)
        {
            if (isCurrentlyAiming)
            {
                // --- PHẦN THÊM MỚI: PHÁT ÂM THANH ---
                if (audioSource != null && laserShootSound != null)
                {
                    // Mẹo nhỏ: Đổi pitch ngẫu nhiên một chút để tiếng súng nghe không bị chán
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(laserShootSound, volume);
                }

                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectTile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            }

            // Reset input bắn dù có bắn được hay không để tránh kẹt phím
            starterAssetsInputs.shoot = false;
        }
    }
}