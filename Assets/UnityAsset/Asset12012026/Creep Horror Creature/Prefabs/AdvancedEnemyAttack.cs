using UnityEngine;
using Script.UI; // Để gọi GameController (nếu cần)

public class AdvancedEnemyAttack : MonoBehaviour
{
    [Header("--- CẤU HÌNH SÁT THƯƠNG ---")]
    public float jumpDamage = 40f;  // Nhảy vồ
    public float handDamage = 20f;  // Đánh tay
    public float biteDamage = 10f;  // Cắn

    [Header("--- CÀI ĐẶT TẦM ĐÁNH ---")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private Transform attackPoint;

    [Header("--- THỜI GIAN GÂY DAME (0.0 - 1.0) ---")]
    [Tooltip("Sát thương bắt đầu tính từ % này của animation")]
    [SerializeField] private float damageStartTime = 0.3f;
    [Tooltip("Kết thúc gây sát thương ở % này")]
    [SerializeField] private float damageEndTime = 0.6f;

    // --- BIẾN NỘI BỘ ---
    private Transform player;
    private Animator enemyAnimator;
    private bool _isAttacking;
    private bool _hasCheckedThisAttack;

    // Hash để tối ưu hiệu năng
    private int _isAttackHash;
    private bool _hasIsAttackParameter;

    void Start()
    {
        _isAttacking = false;
        _hasCheckedThisAttack = false;

        // 1. Tự tìm Player
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // 2. Setup Animator
        enemyAnimator = GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            _isAttackHash = Animator.StringToHash("IsAttack");

            // Kiểm tra xem Animator có parameter "IsAttack" không
            foreach (AnimatorControllerParameter param in enemyAnimator.parameters)
            {
                if (param.name == "IsAttack")
                {
                    _hasIsAttackParameter = true;
                    break;
                }
            }
        }

        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        // Nếu game đang pause hoặc không có animator thì nghỉ
        if (Time.timeScale == 0) return;
        if (enemyAnimator == null) return;

        // Lấy thông tin animation đang chạy
        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);

        // --- NHẬN DIỆN TRẠNG THÁI TẤN CÔNG ---
        // (Dựa vào tên State hoặc Parameter IsAttack)
        bool isAttackParam = _hasIsAttackParameter && enemyAnimator.GetBool(_isAttackHash);

        bool isInAttackState = isAttackParam ||
                               stateInfo.IsName("Attack") ||
                               CheckStateNameContains(stateInfo, "Attack") ||
                               CheckStateNameContains(stateInfo, "Jump") ||
                               CheckStateNameContains(stateInfo, "Bite");

        if (isInAttackState)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _hasCheckedThisAttack = false; // Reset cờ đánh
            }

            // Lấy % thời gian animation (0.0 -> 1.0)
            float currentTime = stateInfo.normalizedTime % 1f;

            // Kiểm tra đúng thời điểm vung tay VÀ chưa gây dame
            if (!_hasCheckedThisAttack && currentTime >= damageStartTime && currentTime <= damageEndTime)
            {
                _hasCheckedThisAttack = true; // Đánh dấu là xong việc

                // Tính damage dựa trên tên animation
                float dmg = CalculateDamage(stateInfo);

                // Kiểm tra khoảng cách và trừ máu
                CheckAttackHit(dmg);
            }
        }
        else
        {
            // Thoát trạng thái tấn công
            if (_isAttacking)
            {
                _isAttacking = false;
                _hasCheckedThisAttack = false;
            }
        }
    }

    // --- HÀM TÍNH SÁT THƯƠNG RIÊNG ---
    private float CalculateDamage(AnimatorStateInfo stateInfo)
    {
        string name = GetCurrentStateName(stateInfo).ToLower();

        if (name.Contains("jump"))
        {
            Debug.Log($"<color=red>NHẢY VỒ! Gây {jumpDamage} HP</color>");
            return jumpDamage;
        }
        else if (name.Contains("bite"))
        {
            Debug.Log($"<color=yellow>CẮN! Gây {biteDamage} HP</color>");
            return biteDamage;
        }
        else
        {
            Debug.Log($"<color=orange>ĐÁNH TAY! Gây {handDamage} HP</color>");
            return handDamage;
        }
    }

    private void CheckAttackHit(float amount)
    {
        if (player == null) return;

        float distance = Vector3.Distance(attackPoint.position, player.position);
        if (distance <= attackRange)
        {
            // Gọi hàm trừ máu bên PlayerStats
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(amount);
            }
        }
    }

    // --- CÁC HÀM PHỤ TRỢ ---
    private bool CheckStateNameContains(AnimatorStateInfo stateInfo, string keyword)
    {
        return GetCurrentStateName(stateInfo).ToLower().Contains(keyword.ToLower());
    }

    private string GetCurrentStateName(AnimatorStateInfo stateInfo)
    {
        if (enemyAnimator == null) return "";
        AnimatorClipInfo[] clips = enemyAnimator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0) return clips[0].clip.name;
        return "";
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}