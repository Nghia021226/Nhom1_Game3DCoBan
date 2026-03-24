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

    [SerializeReference] public BlackboardVariable<float> RevengeDuration = new BlackboardVariable<float>(5f);

    private NavMeshAgent agent;
    private Animator anim;
    private float _nextUpdatePathTime;
    private float _chaseStartTime; 
    private bool hasStartedChase = false;

    protected override Status OnStart()
    {
        if (TargetPlayer.Value == null || IsDetected == null) return Status.Failure;

        agent = GameObject.GetComponent<NavMeshAgent>();
        anim = GameObject.GetComponent<Animator>();

        if (agent == null) return Status.Failure;

        _chaseStartTime = Time.time;

        if (anim != null)
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", true);
        }

        EnemySound soundScript = GameObject.GetComponent<EnemySound>();
        if (soundScript != null) soundScript.PlayAlertSound();

        if (MusicManager.instance != null)
        {
            MusicManager.instance.StartChase(GameObject); 
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

        if (GameManager.instance != null && GameManager.instance.isPlayerHiding)
        {
            StopChasing();
            return Status.Failure;
        }

        if (anim != null && !anim.GetBool("IsRun"))
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", true);
        }

        if (Time.time >= _nextUpdatePathTime)
        {
            agent.SetDestination(TargetPlayer.Value.transform.position);
            _nextUpdatePathTime = Time.time + 0.2f;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return Status.Success;
        }

        float distance = Vector3.Distance(agent.transform.position, TargetPlayer.Value.transform.position);

        bool isStillAngry = (Time.time - _chaseStartTime) < RevengeDuration.Value;

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

        if (anim != null) anim.SetBool("IsRun", false);

        if (IsDetected != null) IsDetected.Value = false;
    }

    protected override void OnEnd()
    {
        if (hasStartedChase && MusicManager.instance != null)
        {
            MusicManager.instance.StopChase(GameObject); 
            hasStartedChase = false;
        }
    }
}