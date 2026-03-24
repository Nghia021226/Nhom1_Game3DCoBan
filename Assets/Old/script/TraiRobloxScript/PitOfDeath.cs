using UnityEngine;

public class PitOfDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            float damageToKill = playerStats.maxHealth;
            playerStats.Die();
        }
    }
}