using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player (Revenge Mode)",
                 story: "Chase Player with initial revenge phase regardless of distance",
                 category: "Action/Navigation",
                 id: "ChasePlayerRevenge_Full")]
public partial class MoveToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetPlayer;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StopChaseDistance;
    [SerializeReference] public BlackboardVariable<bool> IsDetected;

    // Thời gian "cay cú" - dù ở xa vẫn rượt (mặc định 5 giây)
    [SerializeReference] public BlackboardVariable<float> RevengeDuration = new BlackboardVariable<float>(5f);

    private NavMeshAgent agent;
    private Animator anim;
    private float _nextUpdatePathTime;
    private float _chaseStartTime; // Lưu thời điểm bắt đầu rượt
    private bool hasStartedChase = false;

    protected override Status OnStart()
    {
        if (TargetPlayer.Value == null || IsDetected == null) return Status.Failure;

        agent = GameObject.GetComponent<NavMeshAgent>();
        anim = GameObject.GetComponent<Animator>();

        if (agent == null) return Status.Failure;

        // Bắt đầu tính thời gian "cơn giận"
        _chaseStartTime = Time.time;

        // Thiết lập Animation Chạy
        if (anim != null)
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", true);
        }

        // Âm thanh báo động
        EnemySound soundScript = GameObject.GetComponent<EnemySound>();
        if (soundScript != null) soundScript.PlayAlertSound();

        // Bật nhạc rượt đuổi
        if (MusicManager.instance != null)
        {
            MusicManager.instance.StartChase();
            hasStartedChase = true;
        }

        agent.speed = Speed.Value;
        agent.isStopped = false;
        agent.SetDestination(TargetPlayer.Value.transform.position);
        _nextUpdatePathTime = Time.time + 0.2f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null || TargetPlayer.Value == null) return Status.Failure;

        // 1. Kiểm tra nếu người chơi đang trốn
        if (GameManager.instance != null && GameManager.instance.isPlayerHiding)
        {
            StopChasing();
            return Status.Failure;
        }

        // 2. ÉP ANIMATION CHẠY: Đảm bảo quái luôn ở dáng chạy khi đang đuổi
        if (anim != null && !anim.GetBool("IsRun"))
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", true);
        }

        // 3. Cập nhật đường đi theo Player mỗi 0.2s
        if (Time.time >= _nextUpdatePathTime)
        {
            agent.SetDestination(TargetPlayer.Value.transform.position);
            _nextUpdatePathTime = Time.time + 0.2f;
        }

        // 4. Nếu đã đến đủ gần để tấn công (Thành công)
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return Status.Success;
        }

        // 5. LOGIC KIỂM TRA BỎ CUỘC (Có tính Revenge Mode)
        float distance = Vector3.Distance(agent.transform.position, TargetPlayer.Value.transform.position);

        // Kiểm tra xem còn trong thời gian "cay cú" không (ví dụ: trong vòng 5s đầu)
        bool isStillAngry = (Time.time - _chaseStartTime) < RevengeDuration.Value;

        // Chỉ bỏ cuộc nếu: Đã hết thời gian trả thù VÀ khoảng cách hiện tại lớn hơn tầm bỏ cuộc
        if (distance > StopChaseDistance.Value && !isStillAngry)
        {
            StopChasing();
            return Status.Failure;
        }

        return Status.Running;
    }

    private void StopChasing()
    {
        if (agent != null) agent.ResetPath();

        // Tắt animation chạy
        if (anim != null) anim.SetBool("IsRun", false);

        // Reset biến phát hiện để Behavior Graph quay về nhánh Patrol (đi tuần)
        if (IsDetected != null) IsDetected.Value = false;
    }

    protected override void OnEnd()
    {
        // Tắt nhạc rượt đuổi khi hành động này kết thúc
        if (hasStartedChase && MusicManager.instance != null)
        {
            MusicManager.instance.StopChase();
            hasStartedChase = false;
        }
    }
}