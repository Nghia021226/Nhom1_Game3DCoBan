using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging; // Đảm bảo có thư viện này

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimCollierMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectTile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    // KHAI BÁO 2 RIG Ở ĐÂY
    [SerializeField] private Rig rig1; // Kéo Rig tay phải vào đây
    [SerializeField] private Rig rig2;  // Kéo Rig tay trái vào đây

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 mouseWordPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimCollierMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWordPosition = raycastHit.point;
        }

        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            // BẬT CẢ 2 RIG LÊN 1
            float targetWeight = 1f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, targetWeight, Time.deltaTime * 13f);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, targetWeight, Time.deltaTime * 13f);

            // Bật Animator Layer (nếu có)
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), targetWeight, Time.deltaTime * 13f));

            Vector3 wordAimTarget = mouseWordPosition;
            wordAimTarget.y = transform.position.y;
            Vector3 aimDirection = (wordAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);

            // TẮT CẢ 2 RIG VỀ 0
            float targetWeight = 0f;
            if (rig1 != null) rig1.weight = Mathf.Lerp(rig1.weight, targetWeight, Time.deltaTime * 13f);
            if (rig2 != null) rig2.weight = Mathf.Lerp(rig2.weight, targetWeight, Time.deltaTime * 13f);

            // Tắt Animator Layer
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), targetWeight, Time.deltaTime * 13f));
        }

        if (starterAssetsInputs.shoot)
        {
            Vector3 aimDir = (mouseWordPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectTile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }
}