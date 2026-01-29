using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, Unity.Properties.GeneratePropertyBag]
[NodeDescription(name: "Move To Patrol Waypoint (Auto Anim)", story: "Pick random waypoint and move", category: "Action/Navigation", id: "MoveToWaypoint_AutoAnim")]
public partial class MoveToWaypointAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<bool> IsDetected;
    [SerializeReference] public BlackboardVariable<bool> IsDead;

    private NavMeshAgent agent;
    private Animator anim;

    protected override Status OnStart()
    {
        agent = GameObject.GetComponent<NavMeshAgent>();
        anim = GameObject.GetComponent<Animator>();
        EnemyPatrolData patrolData = GameObject.GetComponent<EnemyPatrolData>();

        if (agent == null || patrolData == null) return Status.Failure;

        if (IsDead.Value || IsDetected.Value) return Status.Failure;

        // --- TỰ ĐỘNG BẬT ANIMATION ĐI BỘ ---
        if (anim != null)
        {
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", true);
        }

        Vector3 dest = patrolData.GetRandomWaypoint();
        agent.speed = Speed.Value;
        agent.isStopped = false;
        agent.SetDestination(dest);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null) return Status.Failure;

        if (IsDead.Value || IsDetected.Value)
        {
            agent.ResetPath();
            return Status.Failure;
        }

        // --- PHẦN SỬA QUAN TRỌNG ---
        // Đảm bảo IsWalk luôn đúng khi đang di chuyển tuần tra
        if (anim != null && !anim.GetBool("IsWalk") && agent.remainingDistance > 0.1f)
        {
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", true);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            if (anim != null) anim.SetBool("IsWalk", false);
            return Status.Success;
        }

        return Status.Running;
    }
}