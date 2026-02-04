using UnityEngine;
using Unity.Behavior;

public class MonsterSenses : MonoBehaviour
{
    [Header("Cài đặt")]
    public float detectionRadius = 15f;
    // Bỏ dòng LayerMask playerLayer cũ đi
    public LayerMask foodLayer; // Vẫn giữ layer cho đồ ăn
    public string playerTag = "Player"; // <--- TÌM BẰNG TAG

    private BehaviorGraphAgent behaviorAgent;

    void Start()
    {
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
    }

    void Update()
    {
        if (behaviorAgent == null) return;

        DetectFood();
        DetectPlayerByTag(); // Gọi hàm mới
    }

    // Hàm mới: Tìm Player bằng Tag (Bất chấp Layer nào)
    void DetectPlayerByTag()
    {
        GameObject foundPlayer = null;

        // Quét tất cả vật thể xung quanh trong bán kính
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var hit in hits)
        {
            // Kiểm tra Tag
            if (hit.CompareTag(playerTag))
            {
                foundPlayer = hit.gameObject;
                break; // Tìm thấy rồi thì dừng loop ngay
            }
        }

        // Cập nhật vào Graph (Nếu tìm thấy thì gán Player, không thì gán null)
        if (foundPlayer != null)
        {
            behaviorAgent.SetVariableValue("TargetPlayer", foundPlayer);
        }
        else
        {
            // Dòng này chính là thủ phạm làm mất Player trong Inspector nếu không tìm thấy
            // Nhưng giờ code tìm đúng Tag rồi nên nó sẽ giữ nguyên
            behaviorAgent.SetVariableValue("TargetPlayer", (GameObject)null);
        }
    }

    void DetectFood()
    {
        Collider[] foods = Physics.OverlapSphere(transform.position, detectionRadius, foodLayer);
        if (foods.Length > 0)
        {
            behaviorAgent.SetVariableValue("TargetFood", foods[0].gameObject);
        }
        else
        {
            behaviorAgent.SetVariableValue("TargetFood", (GameObject)null);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}