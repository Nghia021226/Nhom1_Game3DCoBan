using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI; // Cần cái này để tắt NavMesh

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
        if (agent != null) agent.enabled = false; // Tắt AI tìm đường để tự bay

        startPos = GameObject.transform.position;
        targetPos = Target.Value.transform.position;
        timer = 0;

        // Quay mặt về phía mục tiêu trước khi nhảy
        GameObject.transform.LookAt(new Vector3(targetPos.x, GameObject.transform.position.y, targetPos.z));

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        float progress = timer / JumpDuration.Value;

        if (progress >= 1f)
        {
            if (agent != null) agent.enabled = true; // Bật lại AI
            return Status.Success;
        }

        // Di chuyển thẳng tới mục tiêu (Lerp)
        GameObject.transform.position = Vector3.Lerp(startPos, targetPos, progress);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (agent != null) agent.enabled = true;
    }
}