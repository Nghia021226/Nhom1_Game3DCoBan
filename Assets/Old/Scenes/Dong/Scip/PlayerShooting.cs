using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.1f; // Tốc độ bắn cực nhanh
    private float nextFireTime = 0f;

    void Update()
    {
        // GetMouseButton(1) là chuột phải. Giữ chuột là bắn liên tục.
        if (Input.GetMouseButton(1) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}