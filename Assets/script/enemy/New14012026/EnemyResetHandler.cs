using UnityEngine;
using UnityEngine.AI;

public class EnemyResetHandler : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private NavMeshAgent _agent;
    private Animator _anim;

    void Start()
    {
        // 1. Lưu lại vị trí và góc xoay ngay khi game bắt đầu
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    public void ResetEnemy()
    {
        // 2. Dịch chuyển quái về vị trí gốc
        // Lưu ý: Với NavMeshAgent, dùng .Warp sẽ chính xác và không bị lỗi vật lý
        if (_agent != null)
        {
            _agent.Warp(_startPosition);
            _agent.ResetPath(); // Xóa lệnh đuổi cũ
        }
        else
        {
            transform.position = _startPosition;
        }

        transform.rotation = _startRotation;

        // 3. Reset Animation về trạng thái nghỉ (Idle)
        if (_anim != null)
        {
            _anim.Rebind();
            _anim.Update(0f);
        }

        Debug.Log($"Đã Reset quái: {gameObject.name}");
    }
}