using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyData data;
    private float currentHealth;
    private bool isDead = false;
    private Animator anim;
    private BehaviorGraphAgent behaviorAgent;

    void Start()
    {
        anim = GetComponent<Animator>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        if (data != null) currentHealth = data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        // Bật phát hiện ngay lập tức
        if (behaviorAgent != null)
            behaviorAgent.SetVariableValue("IsDetected", true);

        StartCoroutine(HitStunRoutine());

        if (currentHealth <= 0) Die();
    }

    private System.Collections.IEnumerator HitStunRoutine()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        // 1. ÉP TẮT CÁC ANIMATION CŨ ĐỂ "HIT" ĐƯỢC ƯU TIÊN
        if (anim != null)
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", false);
            anim.SetTrigger("Hit");
        }

        // 2. KHỰNG LẠI (0.2 giây là đủ để thấy uy lực súng)
        if (agent != null) agent.isStopped = true;

        yield return new WaitForSeconds(0.2f);

        if (!isDead && agent != null) agent.isStopped = false;
    }

    void Die()
    {
        isDead = true;
        if (anim != null) anim.SetTrigger("Die");
        if (behaviorAgent != null) behaviorAgent.SetVariableValue("IsDead", true);
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().isStopped = true;
        Destroy(gameObject, 5f);
    }
}