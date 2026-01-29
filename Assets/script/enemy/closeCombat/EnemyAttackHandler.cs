using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Script.UI; // Bắt buộc có dòng này để gọi GameController
using UnityEngine.SceneManagement;

namespace Script.Enemy
{
    public class EnemyAttackHandler : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float damageAmount = 20f;

        [Header("Timing Settings")]
        [Tooltip("Thời điểm bắt đầu gây sát thương (0.0 - 1.0)")]
        [SerializeField] private float damageStartTime = 0.35f;

        [Tooltip("Thời điểm kết thúc gây sát thương")]
        [SerializeField] private float damageEndTime = 0.5f;

        //[Header("UI References")]
        //[SerializeField] private GameObject youreDeadPanel;
        //[SerializeField] private Button restartButton;
        //[SerializeField] private Button BackToMenuButton;
        //[SerializeField] private Button QuitButton;
        //[SerializeField] private TMP_Text deathText;

        [Header("References")]
         private Transform player;
         private Animator enemyAnimator;
        //[SerializeField] private Transform playerCheckpoint;
        //[SerializeField] private Transform enemyCheckpoint;

        private bool _hasKilledPlayer;
        private bool _isAttacking;
        private bool _hasCheckedThisAttack;
        private int _isAttackHash;
        private bool _hasIsAttackParameter;

        void Start()
        {
            _hasKilledPlayer = false;
            _isAttacking = false;
            _hasCheckedThisAttack = false;

            // Tự động tìm Player bằng Tag để đỡ phải kéo thả từng con quái
            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (enemyAnimator == null) enemyAnimator = GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                _isAttackHash = Animator.StringToHash("IsAttack");
                foreach (AnimatorControllerParameter param in enemyAnimator.parameters)
                {
                    if (param.name == "IsAttack")
                    {
                        _hasIsAttackParameter = true;
                        enemyAnimator.SetBool(_isAttackHash, false);
                        break;
                    }
                }
            }

            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) player = playerObj.transform;
            }
            if (attackPoint == null) attackPoint = transform;
        }

        void Update()
        {
            if (_hasKilledPlayer) return;
            if (GameController.IsGamePaused()) return;
            if (enemyAnimator == null) return;

            AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);

            bool isAttackParameter = false;
            if (_hasIsAttackParameter)
            {
                isAttackParameter = enemyAnimator.GetBool(_isAttackHash);
            }

            bool isInAttackState = isAttackParameter ||
                                   stateInfo.IsName("Attack") ||
                                   CheckStateNameContains(stateInfo, "attack");

            if (isInAttackState)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    _hasCheckedThisAttack = false;
                }

                float currentTime = stateInfo.normalizedTime % 1f;

                if (!_hasCheckedThisAttack && currentTime >= damageStartTime && currentTime <= damageEndTime)
                {
                    _hasCheckedThisAttack = true;
                    CheckAttackHit();
                }
            }
            else
            {
                if (_isAttacking)
                {
                    _isAttacking = false;
                    _hasCheckedThisAttack = false;
                }
            }
        }

        private bool CheckStateNameContains(AnimatorStateInfo stateInfo, string keyword)
        {
            string stateName = GetCurrentStateName(stateInfo);
            return stateName.ToLower().Contains(keyword.ToLower());
        }

        private string GetCurrentStateName(AnimatorStateInfo stateInfo)
        {
            if (enemyAnimator == null) return "Unknown";
            int layerIndex = 0;
            AnimatorClipInfo[] clipInfos = enemyAnimator.GetCurrentAnimatorClipInfo(layerIndex);
            if (clipInfos.Length > 0) return clipInfos[0].clip.name;
            return $"State_{stateInfo.shortNameHash}";
        }

        private void CheckAttackHit()
        {
            if (IsPlayerInAttackRange()) KillPlayer();
        }

        private bool IsPlayerInAttackRange()
        {
            if (player == null) return false;
            float distanceToPlayer = Vector3.Distance(attackPoint.position, player.position);
            return distanceToPlayer <= attackRange;
        }

        private void KillPlayer()
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damageAmount); // Gây sát thương tối đa để chết luôn
            }
        }
        void ResetAttackState()
        {
            _hasKilledPlayer = false;
        }

        //private void OnRestartButtonClicked()
        //{
        //    // Tắt UI chết chóc
        //    GameController.ClearAllPanels();
        //    if (youreDeadPanel != null) youreDeadPanel.SetActive(false);

        //    // 1. Dịch chuyển Player và Enemy về vị trí cũ
        //    TeleportToCheckpoints();

        //    // 2. --- QUAN TRỌNG: HỒI ĐẦY MÁU CHO PLAYER ---
        //    if (player != null)
        //    {
        //        PlayerStats stats = player.GetComponent<PlayerStats>();
        //        if (stats != null)
        //        {
        //            stats.ResetStats(); // Gọi hàm hồi máu
        //        }
        //    }
        //    // ---------------------------------------------

        //    if (enemyAnimator != null && _hasIsAttackParameter) enemyAnimator.SetBool(_isAttackHash, false);

        //    _hasKilledPlayer = false;
        //    _isAttacking = false;
        //    _hasCheckedThisAttack = false;
        //}

//        public void LoadMainMenu()
//        {
//            SceneManager.LoadScene(0);
//        }

//        public void QuitGame()
//        {
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//                        Application.Quit();
//#endif
//        }

        //private void TeleportToCheckpoints()
        //{
        //    if (player != null && playerCheckpoint != null)
        //    {
        //        CharacterController playerController = player.GetComponent<CharacterController>();
        //        if (playerController != null)
        //        {
        //            playerController.enabled = false;
        //            player.position = playerCheckpoint.position;
        //            player.rotation = playerCheckpoint.rotation;
        //            playerController.enabled = true;
        //        }
        //        else
        //        {
        //            player.position = playerCheckpoint.position;
        //            player.rotation = playerCheckpoint.rotation;
        //        }
        //    }
        //    if (enemyCheckpoint != null)
        //    {
        //        transform.position = enemyCheckpoint.position;
        //        transform.rotation = enemyCheckpoint.rotation;
        //    }
        //}

        private void OnDrawGizmosSelected()
        {
            Transform point = attackPoint != null ? attackPoint : transform;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point.position, attackRange);
            if (player != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(point.position, player.position);
            }
        }
    }
}