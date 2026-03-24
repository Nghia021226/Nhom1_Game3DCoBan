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

        Vector3 eyePosition = GameObject.transform.position + Vector3.up;
        Vector3 playerChestPosition = Target.Value.transform.position + Vector3.up;
        float distanceToPlayer = Vector3.Distance(eyePosition, playerChestPosition);
        Vector3 directionToPlayer = (playerChestPosition - eyePosition).normalized;

        bool seen = false;

        // Sử dụng .Value để lấy giá trị từ BlackboardVariable
        if (distanceToPlayer <= Proximity.Value)
        {
            seen = true;
        }
        else if (distanceToPlayer <= ViewDistance.Value)
        {
            if (Vector3.Angle(GameObject.transform.forward, directionToPlayer) < ViewAngle.Value * 0.5f)
            {
                int wallLayerMask = LayerMask.GetMask(ObstacleLayer.Value);
                if (!Physics.Raycast(eyePosition, directionToPlayer, distanceToPlayer, wallLayerMask))
                {
                    seen = true;
                    // Nếu thấy người chơi: Vẽ tia ĐỎ
                    Debug.DrawRay(eyePosition, directionToPlayer * distanceToPlayer, Color.red);
                }
                else
                {
                    // Nếu bị tường chặn hoặc ngoài tầm nhìn: Vẽ tia XANH
                    Debug.DrawRay(eyePosition, directionToPlayer * distanceToPlayer, Color.green);
                }
                
            }
        }

        if (_eyeLight != null)
        {
            if (seen)
            {
                IsDetected.Value = true;
            }

            // Cập nhật đèn cảnh báo dựa trên trạng thái IsDetected tổng thể
            if (_eyeLight != null)
            {
                _eyeLight.color = IsDetected.Value ? Color.red : Color.green;
                _eyeLight.intensity = IsDetected.Value ? 15f : 3f;
            }
        }

            
        return Status.Running;
    }
}