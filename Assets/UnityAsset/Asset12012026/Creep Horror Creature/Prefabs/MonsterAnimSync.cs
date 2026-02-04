using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimSync : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Lấy tốc độ thực tế của con quái
        float currentSpeed = agent.velocity.magnitude;

        // Gửi vào Animator (Tham số tên là "Speed")
        anim.SetFloat("Speed", currentSpeed);
    }
}