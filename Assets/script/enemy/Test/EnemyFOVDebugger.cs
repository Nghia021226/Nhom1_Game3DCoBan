using UnityEngine;

public class EnemyFOVDebugger : MonoBehaviour
{
    [SerializeField] float viewDistance = 10f;
    [SerializeField] float viewAngle = 90f;
    [SerializeField] Color coneColor = new Color(0, 1, 0, 0.2f); // Màu xanh lá mờ

    // Hàm này sẽ vẽ hình lên cửa sổ Scene
    private void OnDrawGizmos()
    {
        Gizmos.color = coneColor;
        Vector3 pos = transform.position + Vector3.up; // Vẽ từ ngang ngực

        // Vẽ 2 đường biên của hình nón
        Vector3 leftDir = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up) * transform.forward;

        Gizmos.DrawLine(pos, pos + leftDir * viewDistance);
        Gizmos.DrawLine(pos, pos + rightDir * viewDistance);

        // Vẽ một vòng cung nhỏ để tạo thành hình nón (Optional)
        int segments = 10;
        Vector3 previousPos = pos + leftDir * viewDistance;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle * 0.5f + (viewAngle / segments) * i;
            Vector3 nextDir = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            Vector3 nextPos = pos + nextDir * viewDistance;
            Gizmos.DrawLine(previousPos, nextPos);
            previousPos = nextPos;
        }
    }
}