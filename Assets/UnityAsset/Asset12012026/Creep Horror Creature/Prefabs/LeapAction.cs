using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Leap To Target", story: "Leap to [Target]", category: "Action/Combat", id: "LeapAction")]
public partial class LeapAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> JumpDuration = new BlackboardVariable<float>(0.5f);
    [SerializeReference] public BlackboardVariable<float> JumpSpeed = new BlackboardVariable<float>(15f);

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;
    private NavMeshAgent agent;

    protected override Status OnStart()
    {
        if (Target.Value == null) return Status.Failure;

        agent = GameObject.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false; 

        startPos = GameObject.transform.position;
        targetPos = Target.Value.transform.position;
        timer = 0;

        
        GameObject.transform.LookAt(new Vector3(targetPos.x, GameObject.transform.position.y, targetPos.z));

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        float progress = timer / JumpDuration.Value;

        if (progress >= 1f)
        {
            if (agent != null) agent.enabled = true; 
            return Status.Success;
        }

       
        GameObject.transform.position = Vector3.Lerp(startPos, targetPos, progress);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null) agent.enabled = true;
    }
}