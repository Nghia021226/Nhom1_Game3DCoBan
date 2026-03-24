using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimSync : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private int speedHash;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();

        
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (anim == null) anim = GetComponentInParent<Animator>();

        if (anim == null)
        {
            Debug.LogError("❌ LỖI RỒI BRO ƠI: Không tìm thấy Animator nào cả! Check lại model đi!");
            this.enabled = false; 
            return;
        }

        if (agent == null)
        {
            Debug.LogError("❌ LỖI: Không có NavMeshAgent!");
        }

        
        speedHash = Animator.StringToHash("Speed");
    }

    void Update()
    {
        if (agent == null || anim == null) return;

        
        float currentSpeed = agent.velocity.magnitude;

      
        anim.SetFloat(speedHash, currentSpeed);
    }
}