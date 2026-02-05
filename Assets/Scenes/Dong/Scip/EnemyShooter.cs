using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;

    float nextFireTime;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Gọi khi enemy quyết định tấn công
    public void StartAttack()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        anim.SetTrigger("Attack");
    }

    // === ANIMATION EVENT ===
    // Gắn event này vào frame bắn trong animation
    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
