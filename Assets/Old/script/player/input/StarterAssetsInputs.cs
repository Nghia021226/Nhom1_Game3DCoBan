using UnityEngine;
using System.Collections; // Cần thêm cái này để dùng Coroutine
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool steal;
        public bool aim;
        public bool shoot;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        public static bool isGameActive = true;

        void Start()
        {
            // --- FIX QUAN TRỌNG: Đợi 1 frame rồi mới check ---
            // Để tránh việc nút bấm UI ở Menu cướp chuột
            StartCoroutine(DelayedCursorSetup());
        }

        IEnumerator DelayedCursorSetup()
        {
            yield return null; // Đợi 1 khung hình
            CheckSceneState();
        }

        private void CheckSceneState()
        {
            string currentScene = SceneManager.GetActiveScene().name;

            // Kiểm tra: Nếu KHÔNG PHẢI là Menu hay Login -> Thì là Game -> Khóa chuột
            // Bro nhớ thay tên scene Menu/Login cho đúng nếu có đổi
            if (currentScene != "TraiRoblox2" && currentScene != "LoginScene")
            {
                SetGameActive(true); // Vào Game: Khóa chuột, ẩn chuột
            }
            else
            {
                SetGameActive(false); // Ở Menu: Hiện chuột
            }
        }

        // --- Mẹo nhỏ: Nếu lỡ Alt-Tab ra ngoài rồi vào lại game thì khóa lại ---
        private void OnApplicationFocus(bool hasFocus)
        {
            if (isGameActive && hasFocus)
            {
                LockCursor();
            }
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (isGameActive) MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook && isGameActive) LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            if (isGameActive) JumpInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            if (isGameActive) ShootInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            if (isGameActive) SprintInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            if (isGameActive) AimInput(value.isPressed);
        }

        public void OnSteal(InputValue value)
        {
            if (isGameActive) StealInput(value.isPressed);
        }

        public void OnMenu(InputValue value)
        {
            if (SceneManager.GetActiveScene().name == "TraiRoblox2" || SceneManager.GetActiveScene().name == "LoginScene") return;

            if (value.isPressed && isGameActive)
            {
                GameObject gameController = GameObject.Find("GameController");
                if (gameController != null)
                {
                    gameController.SendMessage("Pause", SendMessageOptions.DontRequireReceiver);
                    gameController.SendMessage("ShowPauseMenu", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    UnlockCursor();
                }
            }
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void StealInput(bool newStealState)
        {
            steal = newStealState;
        }

        // --- CÁC HÀM STATIC QUẢN LÝ CHUỘT ---

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isGameActive = false;
        }

        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isGameActive = true;
        }

        public static void SetGameActive(bool active)
        {
            isGameActive = active;
            if (active) LockCursor();
            else UnlockCursor();
        }

        public static bool IsGameActive()
        {
            return isGameActive;
        }
    }
}