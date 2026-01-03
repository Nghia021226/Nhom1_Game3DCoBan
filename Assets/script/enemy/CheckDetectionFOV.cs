using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Detection FOV",
                 story: "Check FOV for [Target] and update [IsDetected]",
                 category: "Action",
                 id: "CheckDetectionFOV_Fixed")]
public partial class CheckDetectionFOV : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsDetected;

    // Khai báo thế này thì mới hiện ở Inspector và chọn được Local Value hoặc Blackboard
    [SerializeReference] public BlackboardVariable<float> ViewDistance = new BlackboardVariable<float>(10f);
    [SerializeReference] public BlackboardVariable<float> ViewAngle = new BlackboardVariable<float>(90f);
    [SerializeReference] public BlackboardVariable<float> Proximity = new BlackboardVariable<float>(2f);
    [SerializeReference] public BlackboardVariable<string> ObstacleLayer = new BlackboardVariable<string>("Default");

    private Light _eyeLight;

    protected override Status OnStart()
    {
        // Tự tìm cái đèn bạn vừa gắn vào con quái
        _eyeLight = GameObject.GetComponentInChildren<Light>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null || IsDetected == null) return Status.Failure;

        Vector3 pos = GameObject.transform.position + Vector3.up;
        Vector3 targetPos = Target.Value.transform.position + Vector3.up;
        float dist = Vector3.Distance(pos, targetPos);
        Vector3 dir = (targetPos - pos).normalized;

        bool seen = false;

        // Sử dụng .Value để lấy giá trị từ BlackboardVariable
        if (dist <= Proximity.Value)
        {
            seen = true;
        }
        else if (dist <= ViewDistance.Value)
        {
            if (Vector3.Angle(GameObject.transform.forward, dir) < ViewAngle.Value * 0.5f)
            {
                int mask = LayerMask.GetMask(ObstacleLayer.Value);
                if (!Physics.Raycast(pos, dir, dist, mask))
                {
                    seen = true;
                    // Nếu thấy người chơi: Vẽ tia ĐỎ
                    Debug.DrawRay(pos, dir * dist, Color.red);
                }
                else
                {
                    // Nếu bị tường chặn hoặc ngoài tầm nhìn: Vẽ tia XANH
                    Debug.DrawRay(pos, dir * dist, Color.green);
                }
                
            }
        }

        if (_eyeLight != null)
        {
            if (seen)
            {
                // Khi thấy người: Chuyển hẳn sang ĐỎ rực
                _eyeLight.color = Color.red;
                _eyeLight.intensity = 15f; // Tăng sáng mạnh để báo động
            }
            else
            {
                // Khi không thấy: Chuyển hẳn sang XANH LÁ (Green)
                // Không để màu đỏ cũ dính vào
                _eyeLight.color = Color.green;
                _eyeLight.intensity = 3f;  // Sáng nhẹ nhàng khi đi tuần
            }
        }

            IsDetected.Value = seen;
        return Status.Success;
    }
}