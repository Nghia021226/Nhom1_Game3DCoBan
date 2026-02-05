using UnityEngine;
using Script.UI;

public class AdvancedEnemyAttack : MonoBehaviour
{
    [Header("--- CẤU HÌNH SÁT THƯƠNG ---")]
    public float jumpDamage = 40f;
    public float handDamage = 20f;
    public float biteDamage = 10f;

    [Header("--- CÀI ĐẶT TẦM ĐÁNH ---")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private Transform attackPoint;

    [Header("--- TIMING (Quan trọng) ---")]
    [Tooltip("Tiếng ĐÁNH + DAME tính từ % này (Ví dụ: 0.3 là vung tay được 30%)")]
    [SerializeField] private float attackTriggerTime = 0.3f;

    [Tooltip("Tiếng ĂN tính từ % này (Chỉnh cao lên để Delay, ví dụ 0.5)")]
    [SerializeField] private float eatTriggerTime = 0.5f; // MỚI: Delay riêng cho Ăn ✅

    // --- BIẾN NỘI BỘ ---
    private Transform player;
    private Animator enemyAnimator;
    private AdvancedEnemySound enemySound;

    private bool _isActionActive;
    private bool _hasTriggeredThisLoop;
    private int _lastLoopCount;

    // Biến để quản lý trạng thái gầm
    private bool _isSuppressingGrowl = false;

    private int _isAttackHash;
    private bool _hasIsAttackParameter;

    void Start()
    {
        _isActionActive = false;
        _hasTriggeredThisLoop = false;
        _lastLoopCount = 0;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        enemyAnimator = GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            _isAttackHash = Animator.StringToHash("IsAttack");
            foreach (AnimatorControllerParameter param in enemyAnimator.parameters)
            {
                if (param.name == "IsAttack") { _hasIsAttackParameter = true; break; }
            }
        }

        enemySound = GetComponent<AdvancedEnemySound>();
        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        if (Time.timeScale == 0 || enemyAnimator == null) return;

        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);

        // --- 1. NHẬN DIỆN ---
        bool isAttackParam = _hasIsAttackParameter && enemyAnimator.GetBool(_isAttackHash);
        bool isAttack = isAttackParam || stateInfo.IsName("Attack") || CheckName(stateInfo, "Attack") || CheckName(stateInfo, "Jump") || CheckName(stateInfo, "Bite");
        bool isEating = CheckName(stateInfo, "Eat");

        // --- 2. XỬ LÝ TẮT TIẾNG GẦM KHI ĂN ---
        if (isEating)
        {
            if (!_isSuppressingGrowl)
            {
                // Mới bắt đầu ăn -> Tắt gầm
                if (enemySound) enemySound.ToggleGrowl(false);
                _isSuppressingGrowl = true;
            }
        }
        else
        {
            if (_isSuppressingGrowl)
            {
                // Vừa ăn xong -> Bật gầm lại
                if (enemySound) enemySound.ToggleGrowl(true);
                _isSuppressingGrowl = false;
            }
        }

        // --- 3. XỬ LÝ LOGIC HÀNH ĐỘNG ---
        if (isAttack || isEating)
        {
            if (!_isActionActive)
            {
                _isActionActive = true;
                _hasTriggeredThisLoop = false;
                _lastLoopCount = (int)stateInfo.normalizedTime;
            }

            // Reset Loop (cho tiếng Ăn lặp lại)
            int currentLoop = (int)stateInfo.normalizedTime;
            if (currentLoop > _lastLoopCount)
            {
                _hasTriggeredThisLoop = false;
                _lastLoopCount = currentLoop;
            }

            // Tính thời gian % animation (0.0 -> 1.0)
            float currentTime = stateInfo.normalizedTime % 1f;

            // --- QUAN TRỌNG: CHỌN MỐC THỜI GIAN RIÊNG ---
            float triggerPoint = isEating ? eatTriggerTime : attackTriggerTime;

            // Kích hoạt (Chỉ chạy 1 lần mỗi loop khi đạt đúng thời điểm)
            if (!_hasTriggeredThisLoop && currentTime >= triggerPoint)
            {
                _hasTriggeredThisLoop = true;

                if (isEating)
                {
                    // Phát tiếng Ăn
                    // Debug.Log("<color=green>NHỒM NHOÀM...</color>");
                    if (enemySound) enemySound.PlayAttackSound("Eat");
                }
                else
                {
                    // Phát tiếng Đánh + Trừ Máu
                    // (Lưu ý: Tiếng đánh được gọi TRƯỚC khi check khoảng cách -> Nên đánh gió vẫn phải kêu)
                    float dmg = CalculateDamageAndSound(stateInfo);
                    CheckAttackHit(dmg);
                }
            }
        }
        else
        {
            if (_isActionActive)
            {
                _isActionActive = false;
                _hasTriggeredThisLoop = false;
            }
        }
    }

    private float CalculateDamageAndSound(AnimatorStateInfo stateInfo)
    {
        string name = GetStateName(stateInfo).ToLower();

        // Code này đảm bảo tiếng kêu LUÔN PHÁT ra dù có trúng hay không
        if (name.Contains("jump"))
        {
            if (enemySound) enemySound.PlayAttackSound("Jump");
            return jumpDamage;
        }
        else if (name.Contains("bite"))
        {
            if (enemySound) enemySound.PlayAttackSound("Bite");
            return biteDamage;
        }
        else
        {
            if (enemySound) enemySound.PlayAttackSound("Hand");
            return handDamage;
        }
    }

    private void CheckAttackHit(float amount)
    {
        if (player == null) return;
        float distance = Vector3.Distance(attackPoint.position, player.position);

        if (distance <= attackRange)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null) stats.TakeDamage(amount);
        }
    }

    private bool CheckName(AnimatorStateInfo info, string key) { return GetStateName(info).ToLower().Contains(key.ToLower()); }

    private string GetStateName(AnimatorStateInfo info)
    {
        if (enemyAnimator == null) return "";
        AnimatorClipInfo[] clips = enemyAnimator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0) return clips[0].clip.name;
        return "";
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
    }
}