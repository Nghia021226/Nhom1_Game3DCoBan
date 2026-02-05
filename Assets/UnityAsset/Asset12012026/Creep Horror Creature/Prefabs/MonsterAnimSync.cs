using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimSync : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private int speedHash;

    void Start()
    {
        // 1. Tìm NavMeshAgent (Ưu tiên tìm trên chính mình trước, rồi tìm ở cha/con)
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();

        // 2. Tìm Animator (Tìm khắp nơi: Bản thân -> Con -> Cha)
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (anim == null) anim = GetComponentInParent<Animator>();

        if (anim == null)
        {
            Debug.LogError("❌ LỖI RỒI BRO ƠI: Không tìm thấy Animator nào cả! Check lại model đi!");
            this.enabled = false; // Tắt script để đỡ spam lỗi
            return;
        }

        if (agent == null)
        {
            Debug.LogError("❌ LỖI: Không có NavMeshAgent!");
        }

        // Đảm bảo tên biến trong Animator đúng là "Speed" (Viết hoa chữ S)
        speedHash = Animator.StringToHash("Speed");
    }

    void Update()
    {
        if (agent == null || anim == null) return;

        // Lấy vận tốc
        float currentSpeed = agent.velocity.magnitude;

        // Đẩy vào Animator
        anim.SetFloat(speedHash, currentSpeed);
    }
}