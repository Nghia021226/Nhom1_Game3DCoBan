using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
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
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimCollierMask))
        {
            // Dòng này sẽ in tên vật thể bị chạm vào Console
            Debug.Log("Ray đang chạm trúng: " + raycastHit.collider.name);
            debugTransform.position = raycastHit.point;
            mouseWordPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            //animator.SetLayerWeight(2, 1f);

            Vector3 wordAimTarget = mouseWordPosition;
            wordAimTarget.y = transform.position.y;
            Vector3 aimDirection = (wordAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward,aimDirection, Time.deltaTime * 20f);   
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            //animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime * 10f));
        }

        if (starterAssetsInputs.shoot)
        {
            //how raycasting works
            //if (hitTransform != null)
            //{

            //    if (hitTransform.GetComponent<bulletTarget>() != null)
            //    {
            //        Instantiate(vfxHitGreen, mouseWordPosition, Quaternion.identity);
            //    }
            //    else
            //    {
            //        Instantiate(vfxHitRed, mouseWordPosition, Quaternion.identity);
            //    }
            //}
            // how Instantiate works
            Vector3 aimDir = (mouseWordPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectTile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }
    }
}
