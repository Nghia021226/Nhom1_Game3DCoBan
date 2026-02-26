using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    [Header("Cài đặt Sát Thương")]
    public float damageAmount = 25f; // Sát thương mỗi lần trúng tia laze

    // Dùng OnTriggerStay để xét sát thương liên tục nếu Player đứng lì bên trong
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // Gọi thẳng TakeDamage. 
                // Script PlayerStats sẽ tự động lo việc chặn dame nếu đang bất tử!
                playerStats.TakeDamage(damageAmount);
            }
        }
    }
}