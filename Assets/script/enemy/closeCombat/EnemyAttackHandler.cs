using UnityEngine;
using Script.UI;

namespace Script.Enemy
{
    public class EnemyAttackHandler : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackRange = 0.8f;
        [Tooltip("Kéo Game Object rỗng ở NẮM ĐẤM vào đây")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float damageAmount = 20f;

        [Header("Animation Settings")]
        [Tooltip("Điền CHÍNH XÁC tên State tấn công trong Animator vào đây")]
        public string attackStateName = "Attack";

        [Header("Tracking Settings")]
        [SerializeField] private float rotationSpeed = 15f;
        [Tooltip("Dừng xoay trước khi đánh bao nhiêu giây?")]
        [SerializeField] private float stopTrackingTime = 0.1f;

        [Header("Timing Settings")]
        [SerializeField] private float damageStartTime = 0.35f;
        [SerializeField] private float damageEndTime = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private Transform player;
        private Animator enemyAnimator;
        private bool _hasKilledPlayer;
        private bool _hasCheckedThisAttack; // Chỉ giữ lại biến này là đủ

        void Start()
        {
            _hasKilledPlayer = false;
            _hasCheckedThisAttack = false;

            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            enemyAnimator = GetComponent<Animator>();

            if (attackPoint == null)
            {
                attackPoint = transform;
                Debug.LogWarning("CHƯA GÁN AttackPoint vào tay! Hitbox sẽ bị tính ở chân.");
            }
        }

        void Update()
        {
            if (_hasKilledPlayer || GameController.IsGamePaused() || enemyAnimator == null) return;

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);

            // Kiểm tra State bằng tên
            bool isInAttackState = stateInfo.IsName(attackStateName);

            if (isInAttackState)
            {
                float currentTime = stateInfo.normalizedTime % 1f;

                // --- TRACKING (XOAY) ---
                float trackingLimit = damageStartTime - stopTrackingTime;
                if (trackingLimit < 0) trackingLimit = damageStartTime;

                if (currentTime < trackingLimit && player != null)
                {
                    RotateTowardsPlayer();
                }

                // --- GÂY DAMAGE ---
                if (!_hasCheckedThisAttack && currentTime >= damageStartTime && currentTime <= damageEndTime)
                {
                    _hasCheckedThisAttack = true;
                    CheckAttackHit();
                }

                // Reset an toàn khi hết animation
                if (currentTime > 0.95f)
                {
                    _hasCheckedThisAttack = false;
                }
            }
            else
            {
                // Reset ngay lập tức khi thoát khỏi trạng thái đánh
                _hasCheckedThisAttack = false;
            }
        }

        private void RotateTowardsPlayer()
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }

        private void CheckAttackHit()
        {
            Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange);
            foreach (var hit in hitPlayers)
            {
                if (hit.CompareTag("Player"))
                {
                    if (showDebugLogs) Debug.Log("Đấm trúng Player!");
                    KillPlayer();
                    return;
                }
            }
            if (showDebugLogs) Debug.Log("Đấm trượt!");
        }

        private void KillPlayer()
        {
            if (player == null) return;
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damageAmount);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Transform point = attackPoint != null ? attackPoint : transform;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point.position, attackRange);
        }
    }
}