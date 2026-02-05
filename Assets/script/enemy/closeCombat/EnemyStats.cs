using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using System.Collections;

public class EnemyStats : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyData data;
    private float currentHealth;
    private bool isDead = false;
    private Animator anim;
    private BehaviorGraphAgent behaviorAgent;

    [Header("Loot Settings")]
    [SerializeField] private GameObject ammoLootPrefab;
    [SerializeField] private float lootHeightOffset = 0.5f;
    [SerializeField] private float lootSpawnDelay = 1.5f;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnTime = 10f;
    [SerializeField] private EnemyPatrolData patrolData;
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    void Start()
    {
        spawnPos = transform.position;
        spawnRot = transform.rotation;

        anim = GetComponent<Animator>();
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
        if (data != null) currentHealth = data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (behaviorAgent != null)
            behaviorAgent.SetVariableValue("IsDetected", true);

        StartCoroutine(HitStunRoutine());

        if (currentHealth <= 0) Die();
    }

    private IEnumerator HitStunRoutine()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (anim != null)
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", false);
            anim.SetTrigger("Hit");
        }

        // Kiểm tra an toàn trước khi dừng
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;

        yield return new WaitForSeconds(0.2f);

        if (!isDead && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("Die");

        // ÉP DỪNG NHẠC TRUY ĐUỔI NGAY LẬP TỨC KHI CHẾT
        if (MusicManager.instance != null)
        {
            MusicManager.instance.StopChase(gameObject); // Báo cho MusicManager biết con quái này đã "nghỉ hưu"
        }

        // 1. TẮT NÃO NGAY LẬP TỨC
        if (behaviorAgent != null)
        {
            behaviorAgent.SetVariableValue("IsDead", true);
            behaviorAgent.enabled = false;
        }

        // 2. DỪNG AGENT AN TOÀN
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // 3. VÔ HIỆU HÓA THÂN THỂ
        if (agent != null) agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(SpawnLootRoutine());
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator SpawnLootRoutine()
    {
        yield return new WaitForSeconds(lootSpawnDelay);

        if (ammoLootPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * lootHeightOffset;
            Instantiate(ammoLootPrefab, spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(5f);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        transform.position = new Vector3(0, -100, 0);

        yield return new WaitForSeconds(respawnTime);

        // CHỌN VỊ TRÍ HỒI SINH TỪ PATROL DATA
        if (patrolData != null && patrolData.patrolPoints != null && patrolData.patrolPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, patrolData.patrolPoints.Count);
            Transform targetPoint = patrolData.patrolPoints[randomIndex];

            if (targetPoint != null)
            {
                transform.position = targetPoint.position;
                transform.rotation = targetPoint.rotation;
            }
        }
        else
        {
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }

        currentHealth = data.maxHealth;
        isDead = false;

        GetComponent<Collider>().enabled = true;

        // BẬT LẠI THÂN THỂ
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        // BẬT LẠI NÃO
        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = true;
            behaviorAgent.SetVariableValue("IsDead", false);
        }

        if (anim != null) anim.Play("Idle");

        Debug.Log($"{gameObject.name} đã hồi sinh thành công!");
    }
}