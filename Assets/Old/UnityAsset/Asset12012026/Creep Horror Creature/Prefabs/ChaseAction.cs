using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase Player Custom", story: "Chase [Target] within [StopDistance]", category: "Action/Navigation", id: "ChaseAction")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> StopDistance = new BlackboardVariable<float>(2f);
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(5f);

    private NavMeshAgent agent;

    protected override Status OnStart()
    {
        if (Target.Value == null) return Status.Failure;
        agent = GameObject.GetComponent<NavMeshAgent>();
        if (agent == null) return Status.Failure;

        agent.speed = Speed.Value;
        agent.stoppingDistance = StopDistance.Value;
        agent.isStopped = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null) return Status.Failure;

        agent.SetDestination(Target.Value.transform.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return Status.Success; 
        }

        return Status.Running;
    }
}