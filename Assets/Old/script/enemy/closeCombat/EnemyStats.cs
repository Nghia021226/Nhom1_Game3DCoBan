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

    [Header("Combat Settings")]
    [SerializeField] private int maxStunTimes = 2; 
    [SerializeField] private float stunResetTime = 5f; 
    private int currentStunCount = 0;
    private float lastHitTime = 0f;

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

        if (Time.time > lastHitTime + stunResetTime)
        {
            currentStunCount = 0;
        }
        lastHitTime = Time.time;

        if (currentStunCount < maxStunTimes)
        {
            currentStunCount++;
            StartCoroutine(HitStunRoutine());
        }
        else
        {
            Debug.Log("Quái đang nổi điên! Không bị choáng nữa!");
        }
     
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

        if (MusicManager.instance != null)
        {
            MusicManager.instance.StopChase(gameObject);
        }

        if (behaviorAgent != null)
        {
            behaviorAgent.SetVariableValue("IsDead", true);
            behaviorAgent.enabled = false;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

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
        currentStunCount = 0; 
        isDead = false;

        GetComponent<Collider>().enabled = true;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        if (behaviorAgent != null)
        {
            behaviorAgent.enabled = true;
            behaviorAgent.SetVariableValue("IsDead", false);
        }

        if (anim != null) anim.Play("Idle");

        Debug.Log($"{gameObject.name} đã hồi sinh thành công!");
    }
}