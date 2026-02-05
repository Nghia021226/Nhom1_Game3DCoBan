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

    public void Shoot()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        anim.SetBool("Attack", true);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Invoke(nameof(ResetAttack), 0.2f);
    }

    void ResetAttack()
    {
        anim.SetBool("Attack", false);
    }
}
