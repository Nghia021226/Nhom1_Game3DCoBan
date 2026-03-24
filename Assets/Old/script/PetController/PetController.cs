using UnityEngine;

public class PetController : MonoBehaviour
{
    [Header("Trạng thái")]
    public bool isTamed = false;

    [Header("Di chuyển")]
    public Transform player;
    public Vector3 followOffset = new Vector3(0.8f, 1.5f, -1f);
    public float smoothSpeed = 3f;

    [Header("Chiến đấu")]
    public float detectRange = 10f;
    public float fireCooldown = 20f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public LayerMask enemyLayer;

    [Header("Âm thanh (Mới)")]
    public AudioSource audioSource;
    public AudioClip shootSound; // Kéo file âm thanh bắn vào đây

    private float cooldownTimer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Tự động thêm AudioSource nếu quên gắn
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // Âm thanh 3D (nghe xa nhỏ dần)
    }

    public void TamePet()
    {
        isTamed = true;
        Debug.Log("Pet đã được thuần phục! Sẵn sàng chiến đấu.");
    }

    void Update()
    {
        if (!isTamed) return;

        FollowPlayer();
        HandleCombat();
    }

    void FollowPlayer()
    {
        if (player == null) return;
        Vector3 targetPos = player.TransformPoint(followOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, smoothSpeed * Time.deltaTime);
    }

    void HandleCombat()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, detectRange, enemyLayer);

        if (enemies.Length > 0)
        {
            Transform targetEnemy = enemies[0].transform;
            Shoot(targetEnemy);
            cooldownTimer = fireCooldown;
        }
    }

    void Shoot(Transform target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        // --- PHÁT ÂM THANH BẮN ---
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        // -------------------------

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        PetBullet bulletScript = bulletGO.GetComponent<PetBullet>();

        if (bulletScript != null)
        {
            bulletScript.Seek(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}