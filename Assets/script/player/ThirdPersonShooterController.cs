using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserShootSound;
    [SerializeField] private AudioClip dryFireSound;
    [Range(0f, 2f)][SerializeField] private float volume = 1f;

    [Header("Animation Rigging")]
    [SerializeField] private Rig rig1;
    [SerializeField] private Rig rig2;

    [Header("UI Settings")]
    [SerializeField] private GameObject crosshairUI; // Tâm ngắm - Chỉ hiện khi nhắm chuột phải
    [SerializeField] private GameObject gunUIGroup;  // Nhóm UI đạn - Hiện khi đã nhấn Tab (Armed)
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadProgressCircle; // Vòng tròn reload - Chỉ hiện khi đang chạy Coroutine nạp đạn

    [Header("Ammo Settings")]
    [SerializeField] private int currentAmmo = 20;
    [SerializeField] private int maxAmmo = 20;

    [Header("Reload Settings")]
    [SerializeField] private int reserveAmmo = 60;
    [SerializeField] private float reloadTime = 2f;
    private bool isReloading = false;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 5f;
    private float nextTimeToFire = 0f;

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

        // Đảm bảo vòng nạp đạn ẩn lúc bắt đầu game
        if (reloadProgressCircle != null) reloadProgressCircle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 1. Kiểm tra trạng thái súng (Armed = Đang cầm trên tay sau khi bấm Tab)
        bool hasWeaponArmed = combatController != null && combatController.GetIsArmed();

        // HIỆN UI đạn ngay khi cầm súng, ẨN khi không cầm súng
        if (gunUIGroup != null) gunUIGroup.SetActive(hasWeaponArmed);

        // Cập nhật text lượng đạn
        if (ammoText != null)
        {
            ammoText.text = $"<size=120%>{currentAmmo}</size> <size=80%>/ {reserveAmmo}</size>";

            // Đổi sang màu đỏ rực khi còn dưới 5 viên
            ammoText.color = (currentAmmo <= 3) ? Color.red : Color.white;
        } 

        if (isReloading) return;

        // 2. Nạp đạn thủ công bằng phím R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        // 3. Logic Nhắm (Aim)
        bool isCurrentlyAiming = starterAssetsInputs.aim && hasWeaponArmed;

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimCollierMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (isCurrentlyAiming)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            // CHỈ HIỆN TÂM NGẮM KHI ĐANG NHẮM (Chuột phải)
            if (crosshairUI != null) crosshairUI.SetActive(true);

            float lerpSpeed = Time.deltaTime * 13f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, 1f, lerpSpeed);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, 1f, lerpSpeed);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, lerpSpeed));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);

            // ẨN TÂM NGẮM KHI KHÔNG NHẮM
            if (crosshairUI != null) crosshairUI.SetActive(false);

            float lerpSpeed = Time.deltaTime * 13f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, 0f, lerpSpeed);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, 0f, lerpSpeed);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, lerpSpeed));
        }

        // 4. Xử lý Bắn (Shoot)
        if (starterAssetsInputs.shoot && isCurrentlyAiming && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            if (currentAmmo > 0)
            {
                currentAmmo--;
                if (audioSource != null && laserShootSound != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(laserShootSound, volume);
                }

                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
                Instantiate(pfBulletProjectTile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            }
            else
            {
                // TỰ ĐỘNG NẠP ĐẠN KHI HẾT ĐẠN MÀ VẪN BẤM BẮN
                if (reserveAmmo > 0)
                {
                    StartCoroutine(ReloadRoutine());
                }
                else
                {
                    // Laze xịt nếu hết cả đạn dự trữ
                    if (audioSource != null && dryFireSound != null && !audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(dryFireSound, volume);
                    }
                }
            }
            starterAssetsInputs.shoot = false;
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // CHỈ HIỆN VÒNG TRÒN RELOAD KHI BẮT ĐẦU NẠP
        if (reloadProgressCircle != null)
        {
            reloadProgressCircle.gameObject.SetActive(true);
            reloadProgressCircle.fillAmount = 0;
        }

        float timer = 0f;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            if (reloadProgressCircle != null) reloadProgressCircle.fillAmount = timer / reloadTime;
            yield return null;
        }

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToTransfer = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToTransfer;
        reserveAmmo -= ammoToTransfer;

        // ẨN VÒNG TRÒN RELOAD KHI XONG
        if (reloadProgressCircle != null) reloadProgressCircle.gameObject.SetActive(false);
        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log($"Đã nạp thêm {amount} đạn dự trữ.");
    }
}