using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    [Header("Cài đặt Sát Thương")]
    [SerializeField] float damageAmount = 25f;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageAmount);
            }
        }
    }
}