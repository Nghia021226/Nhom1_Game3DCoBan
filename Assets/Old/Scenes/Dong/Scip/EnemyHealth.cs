using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    private int currentHealth;
    private bool isDead;

    private Animator anim;
    private NavMeshAgent agent;
    [Header("Hit Settings")]
    public float hitCooldown = 0.2f;
    private float lastHitTime;


    void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (!other.CompareTag("PlayerBullet")) return;

        BulletMover bullet = other.GetComponent<BulletMover>();
        int damage = bullet != null ? bullet.damage : 10;

        TakeDamage(damage);
        Destroy(other.gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // ===== HIT CÓ COOLDOWN =====
        if (Time.time - lastHitTime >= hitCooldown)
        {
            lastHitTime = Time.time;

            anim.ResetTrigger("Hit");
            anim.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Invoke(nameof(Die), 0.1f);
        }
    }

    // ===== ANIMATION EVENT =====
    public void Hit_Start()
    {
        if (agent != null && agent.enabled)
            agent.isStopped = true;
    }

    public void Hit_End()
    {
        if (agent != null && agent.enabled)
            agent.isStopped = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("Die");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        foreach (var s in GetComponents<MonoBehaviour>())
            if (s != this) s.enabled = false;

        
    }
}
