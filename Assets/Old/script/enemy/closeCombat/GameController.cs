using UnityEngine;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Script.UI
{
    public class GameController : MonoBehaviour
    {
        
        private static GameController _instance;
        public static GameController Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameController");
                    _instance = go.AddComponent<GameController>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Death UI References (Cập nhật)")]
        [SerializeField] private GameObject deathPanel;
        [SerializeField] private TMP_Text deathText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton; // Thêm nút về Menu
        [SerializeField] private Button quitButton; // Thêm nút Thoát game

        private bool _isGamePaused;
        private List<GameObject> _activePanels = new List<GameObject>();

        
        private Dictionary<Animator, bool> _animatorStates = new Dictionary<Animator, bool>();
        private Dictionary<Rigidbody, bool> _rigidbodyStates = new Dictionary<Rigidbody, bool>();
        private Dictionary<CharacterController, bool> _characterControllerStates = new Dictionary<CharacterController, bool>();
        private Dictionary<MonoBehaviour, bool> _scriptStates = new Dictionary<MonoBehaviour, bool>();

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Thiết lập sự kiện cho các nút
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);

            if (menuButton != null)
                menuButton.onClick.AddListener(LoadMainMenu);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        public static void ShowDeathScreen(string message)
        {
            if (Instance.deathPanel != null)
            {
                if (Instance.deathText != null) Instance.deathText.text = message;
                PauseGame(Instance.deathPanel); // Sử dụng hàm Pause có sẵn
            }
        }

        private void RestartGame()
        {
            // Gọi logic hồi sinh từ GameManager (sẽ viết ở Bước 2)
            if (GameManager.instance != null)
            {
                ClearAllPanels(); // Tắt bảng chết
                GameManager.instance.RestartFromCheckpoint();
            }
        }


        public static bool IsGamePaused()
        {
            return Instance._isGamePaused;
        }

        public static void PauseGame(GameObject panel)
        {
            
            if (panel != null && !Instance._activePanels.Contains(panel))
            {
                Instance._activePanels.Add(panel);
                panel.SetActive(true);
            }

            if (Instance._isGamePaused) return;
            Instance._isGamePaused = true;

            // 2. Mở khóa chuột để bấm nút
            SetInputState(false);

            // 3. Đóng băng thế giới game
            DisableAllAnimators();
            DisableAllRigidbodies();
            DisableAllCharacterControllers();
        }

        public static void ResumeGame(GameObject panel = null)
        {
            
            if (panel != null && Instance._activePanels.Contains(panel))
            {
                Instance._activePanels.Remove(panel);
                panel.SetActive(false);
            }

            
            if (Instance._activePanels.Count == 0)
            {
                Instance._isGamePaused = false;

                // Khóa chuột lại để chơi tiếp
                SetInputState(true);

                EnableAllAnimators();
                EnableAllRigidbodies();
                EnableAllCharacterControllers();
                EnableGameplayScripts();
            }
        }

        public static void ClearAllPanels()
        {
            
            foreach (GameObject panel in Instance._activePanels)
            {
                if (panel != null) panel.SetActive(false);
            }

            Instance._activePanels.Clear();
            Instance._isGamePaused = false;

            // Khóa chuột lại để chơi tiếp
            SetInputState(true);

            EnableAllAnimators();
            EnableAllRigidbodies();
            EnableAllCharacterControllers();
            EnableGameplayScripts();
        }

        public void LoadMainMenu()
        {
            // Quan trọng: Reset lại thời gian trước khi chuyển cảnh 
            // để tránh lỗi đứng hình ở scene sau
            Time.timeScale = 1f;
            SceneManager.LoadScene(0); // Load scene index 0 (thường là MainMenu)
        }

        public void QuitGame()
        {
            Debug.Log("Đã thoát game!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }


        private static void SetInputState(bool isGameActive)
        {
            // Xử lý chuột: Active = Khóa chuột (chơi), !Active = Hiện chuột (menu)
            if (isGameActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            
            var input = FindFirstObjectByType<StarterAssetsInputs>();
            if (input != null)
            {
                
                input.cursorInputForLook = isGameActive;
                input.cursorLocked = isGameActive;
                
            }
        }

        

        private static void DisableAllAnimators()
        {
            Instance._animatorStates.Clear();
            Animator[] animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
            foreach (Animator anim in animators)
            {
                // Không tắt animator của UI
                if (anim.gameObject.activeInHierarchy && !IsUIComponent(anim.gameObject))
                {
                    Instance._animatorStates[anim] = anim.enabled;
                    anim.speed = 0f; // Dừng animation tại chỗ
                }
            }
        }

        private static void EnableAllAnimators()
        {
            foreach (var kvp in Instance._animatorStates)
            {
                if (kvp.Key != null) kvp.Key.speed = 1f; // Chạy lại bình thường
            }
            Instance._animatorStates.Clear();
        }

        private static void DisableAllRigidbodies()
        {
            Instance._rigidbodyStates.Clear();
            Rigidbody[] rigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            foreach (Rigidbody rb in rigidbodies)
            {
                if (!IsUIComponent(rb.gameObject))
                {
                    Instance._rigidbodyStates[rb] = rb.isKinematic;
                    rb.isKinematic = true; // Dừng vật lý rơi/bay
                }
            }
        }

        private static void EnableAllRigidbodies()
        {
            foreach (var kvp in Instance._rigidbodyStates)
            {
                if (kvp.Key != null) kvp.Key.isKinematic = kvp.Value;
            }
            Instance._rigidbodyStates.Clear();
        }

        private static void DisableAllCharacterControllers()
        {
            Instance._characterControllerStates.Clear();
            CharacterController[] controllers = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            foreach (CharacterController cc in controllers)
            {
                if (!IsUIComponent(cc.gameObject))
                {
                    Instance._characterControllerStates[cc] = cc.enabled;
                    cc.enabled = false;
                }
            }
        }

        private static void EnableAllCharacterControllers()
        {
            foreach (var kvp in Instance._characterControllerStates)
            {
                if (kvp.Key != null) kvp.Key.enabled = kvp.Value;
            }
            Instance._characterControllerStates.Clear();
        }

        // Tạm để trống hàm script để tránh lỗi logic game của bạn, 
        // vì disable nhầm script quan trọng sẽ gây bug.
        private static void DisableGameplayScripts() { }
        private static void EnableGameplayScripts() { }

        private static bool IsUIComponent(GameObject obj)
        {
            return obj.GetComponentInParent<Canvas>() != null;
        }
    }
}