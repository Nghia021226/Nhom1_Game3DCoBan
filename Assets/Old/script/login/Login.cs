using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement; // Bắt buộc có để chuyển cảnh

[Serializable]
public class LoginRequest
{
    public string userName;
    public string password;
}

[Serializable]
public class LoginResponse
{
    public int id;
    public string userName;
    public string fullName;
    public int roleID;
    public string roleName;
    public string token;
    public string message;
}

public class Login : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool isDebugMode = false; // Tích chọn cái này trong Inspector để skip Login
    public static bool SkipLogin = false; // Biến static để Menu gọi tới

    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private GameObject loginPanel;

    [Header("Optional UI Feedback")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject loadingPanel;

    [Header("API Configuration")]
    [SerializeField] private string apiUrl = "https://localhost:7237/api/Auth/Login";

    [Header("Scene Configuration (Quan trọng)")]
    [SerializeField] private string menuSceneName = "TraiRoblox2"; // Tên Scene Menu của bro

    private const string TOKEN_KEY = "JWT_TOKEN";
    private const string USER_ID_KEY = "USER_ID";
    private const string USERNAME_KEY = "USERNAME";
    private const string ROLE_KEY = "USER_ROLE";

    private bool isLoggingIn = false;

    private void Awake()
    {
        // Gán giá trị từ Inspector vào biến static khi bắt đầu
        SkipLogin = isDebugMode;
    }

    void Start()
    {
        // Mở khóa chuột để bấm Login
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1.0f;

        // Setup nút bấm
        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        // Ẩn/Hiện UI dựa trên trạng thái đăng nhập
        if (loadingPanel != null) loadingPanel.SetActive(false);

        if (IsLoggedIn()) ShowLoggedInUI();
        else ShowLoginUI();
    }

    public void OnLoginButtonClicked()
    {
        if (isLoggingIn) return;
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Vui lòng nhập tên và mật khẩu!", true);
            return;
        }

        StartCoroutine(LoginCoroutine(username, password));
    }

    public void OnContinueButtonClicked()
    {
        if (!IsLoggedIn())
        {
            ShowStatus("Không tìm thấy tài khoản đã lưu!", true);
            return;
        }

        // Đã đăng nhập trước đó -> Vào thẳng Menu
        LoadMenuScene();
    }

    // --- HÀM CHUYỂN CẢNH SANG MENU ---
    private void LoadMenuScene()
    {
        ShowStatus("Đăng nhập thành công! Đang vào Menu...", false);
        SceneManager.LoadScene(menuSceneName);
    }
    // ---------------------------------

    private IEnumerator LoginCoroutine(string username, string password)
    {
        isLoggingIn = true;
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (loginButton != null) loginButton.interactable = false;

        LoginRequest loginData = new LoginRequest { userName = username, password = password };
        string jsonData = JsonUtility.ToJson(loginData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "text/plain");
            request.certificateHandler = new AcceptAllCertificates();
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (loadingPanel != null) loadingPanel.SetActive(false);
            if (loginButton != null) loginButton.interactable = true;

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);

                if (response != null && !string.IsNullOrEmpty(response.token))
                {
                    // Lưu thông tin
                    PlayerPrefs.SetString(TOKEN_KEY, response.token);
                    PlayerPrefs.SetInt(USER_ID_KEY, response.id);
                    PlayerPrefs.SetString(USERNAME_KEY, response.userName);
                    PlayerPrefs.SetString(ROLE_KEY, response.roleName);
                    PlayerPrefs.Save();

                    // --- CHUYỂN CẢNH ---
                    LoadMenuScene();
                }
                else
                {
                    ShowStatus("Lỗi: Server không trả về Token", true);
                }
            }
            else
            {
                ShowStatus("Đăng nhập thất bại! Kiểm tra lại tài khoản.", true);
            }
        }
        isLoggingIn = false;
    }

    private void ShowLoginUI()
    {
        if (loginPanel != null) loginPanel.SetActive(true);
        if (usernameInput != null) usernameInput.gameObject.SetActive(true);
        if (passwordInput != null) passwordInput.gameObject.SetActive(true);
        if (loginButton != null) loginButton.gameObject.SetActive(true);
        if (continueButton != null) continueButton.gameObject.SetActive(IsLoggedIn());
        if (statusText != null) statusText.text = "";
    }

    private void ShowLoggedInUI()
    {
        string username = GetUsername();
        if (loginPanel != null) loginPanel.SetActive(true);
        if (usernameInput != null) usernameInput.gameObject.SetActive(true); // Cho phép đăng nhập acc khác
        if (passwordInput != null) passwordInput.gameObject.SetActive(true);
        if (loginButton != null) loginButton.gameObject.SetActive(true);
        if (continueButton != null) continueButton.gameObject.SetActive(true);

        if (statusText != null)
        {
            statusText.text = $"Xin chào {username}! Nhấn 'Continue' để vào game.";
            statusText.color = Color.green;
        }
    }

    private void ShowStatus(string message, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : Color.green;
        }
    }

    // Các hàm Static tiện ích
    public static string GetStoredToken() => PlayerPrefs.GetString(TOKEN_KEY, "");
    public static bool IsLoggedIn()
    {
        // --- ĐOẠN CODE FIX: CHỈ CHẠY TRONG UNITY EDITOR ---
#if UNITY_EDITOR
        // Lấy tên Scene đang chạy hiện tại
        string currentScene = SceneManager.GetActiveScene().name;

        // Nếu Scene KHÔNG PHẢI là "LoginScene" (Login) 
        // VÀ KHÔNG PHẢI là "TraiRoblox2" (Menu)
        // -> Suy ra bạn đang test Map Game -> Cho qua luôn (coi như đã Login)
        if (currentScene != "LoginScene" && currentScene != "TraiRoblox2")
        {
            return true;
        }
#endif
        // --------------------------------------------------

        // Nếu đang chạy bản Build thật hoặc đang ở scene Login/Menu thì chạy logic cũ
        if (SkipLogin) return true;

        return !string.IsNullOrEmpty(GetStoredToken());
    }
    public static string GetUsername() => PlayerPrefs.GetString(USERNAME_KEY, "");

    public static void Logout()
    {
        // Cực kỳ quan trọng: Reset lại chế độ Debug khi người dùng chủ động thoát
        SkipLogin = false;

        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey(USER_ID_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        PlayerPrefs.DeleteKey(ROLE_KEY);
        PlayerPrefs.Save();
    }
}

// Giữ nguyên class xử lý chứng chỉ SSL
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) { return true; }
}