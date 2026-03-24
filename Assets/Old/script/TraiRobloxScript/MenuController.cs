using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Cấu hình Scene")]
    public string gameSceneName = "MainMap";    // Scene Game chính
    public string loginSceneName = "LoginScene"; // Scene Login (Bro nhớ tạo Scene này)

    [Header("UI Panels")]
    public GameObject optionsPanel;

    [Header("Settings")]
    public TextMeshProUGUI volumeText;
    private float currentVolume = 1.0f;

    [Header("Speaker Settings")]
    public Image speakerImage;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    private bool isMuted = false;

    void Start()
    {
        // --- BƯỚC BẢO MẬT: CHƯA ĐĂNG NHẬP THÌ KHÔNG ĐƯỢC Ở MENU ---
        if (!Login.IsLoggedIn())
        {
            Debug.LogWarning("Chưa đăng nhập! Quay về màn hình Login.");
            SceneManager.LoadScene(loginSceneName);
            return;
        }
        // ---------------------------------------------------------

        // Đảm bảo chuột hiện lên để bấm menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        currentVolume = AudioListener.volume;
        UpdateVolumeUI();
    }

    public void PlayGame()
    {
        // Vào game chính
        SceneManager.LoadScene(gameSceneName);
    }

    public void Logout()
    {
        // Xóa token và quay về login
        Login.Logout();
        SceneManager.LoadScene(loginSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit();
    }

    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void IncreaseVolume()
    {
        currentVolume += 0.1f;
        if (currentVolume > 1.0f) currentVolume = 1.0f;
        ApplyVolume();
    }

    public void DecreaseVolume()
    {
        currentVolume -= 0.1f;
        if (currentVolume < 0.0f) currentVolume = 0.0f;
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        AudioListener.volume = currentVolume;
        UpdateVolumeUI();
    }

    private void UpdateVolumeUI()
    {
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(currentVolume * 100) + "%";
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            AudioListener.volume = 0;
            if (speakerImage != null && soundOffIcon != null) speakerImage.sprite = soundOffIcon;
        }
        else
        {
            AudioListener.volume = 1;
            if (speakerImage != null && soundOnIcon != null) speakerImage.sprite = soundOnIcon;
        }
    }
}