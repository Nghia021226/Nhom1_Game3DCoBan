using UnityEngine;
using System.Collections.Generic;
using TMPro; // Bắt buộc có để chỉnh chữ UI

public class PowerManager : MonoBehaviour
{
    public static PowerManager instance;

    [Header("UI Hiển thị")]
    [SerializeField] TextMeshProUGUI powerTimerText; 

    [Header("Cài đặt Thời gian (Giây)")]
    [SerializeField] float firstTimeDuration = 60f;   
    [SerializeField] float normalCycleDuration = 180f; 
    [SerializeField] float warningThreshold = 60f;     

    [Header("Danh sách đèn (Tự tìm)")]
    [SerializeField] List<Light> mapLights = new List<Light>();

    [Header("Âm thanh")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip powerDownSFX;
    [SerializeField] AudioClip powerUpSFX;

    [Header("Liên kết Quest (Kéo Cầu dao vào đây)")]
    public Breaker mainBreaker; 

    public bool isPowerOff = false;
    private float currentTime; // Biến đếm thời gian thực tế
    

    void Awake() { if (instance == null) instance = this; }

    void Start()
    {
        // TÌM ĐÈN 
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var l in allLights)
        {
            if (l.GetComponentInParent<PlayerStats>() != null) continue;
            if (l.GetComponentInParent<EnemySound>() != null) continue;
            if (l.GetComponentInParent<EnemyPatrolData>() != null) continue;
            if (l.bakingOutput.lightmapBakeType == LightmapBakeType.Baked) continue;
            mapLights.Add(l);
        }

        // THIẾT LẬP THỜI GIAN BAN ĐẦU
        currentTime = firstTimeDuration; 
        
    }

    void Update()
    {
        // Nếu điện đang tắt thì không đếm giờ nữa (hoặc hiện thông báo khác)
        if (isPowerOff)
        {
            if (powerTimerText != null)
            {
                powerTimerText.text = "NGUỒN ĐIỆN: 00:00";
                powerTimerText.color = Color.red;
            }
            return;
        }

       
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Trừ thời gian theo từng khung hình

            UpdateTimerUI();
        }
        else
        {
            currentTime = 0;
            CutPower();
        }
    }

    void UpdateTimerUI()
    {
        if (powerTimerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        powerTimerText.text = string.Format("NGUỒN ĐIỆN: {0:00}:{1:00}", minutes, seconds);

        if (currentTime <= warningThreshold)
        {
            powerTimerText.color = Color.red; 
        }
        else
        {
            powerTimerText.color = Color.green; 
        }
    }

    public void CutPower()
    {
        if (isPowerOff) return;
        isPowerOff = true;

        if (audioSource && powerDownSFX) audioSource.PlayOneShot(powerDownSFX);
        foreach (var l in mapLights) if (l != null) l.enabled = false;

        if (GameManager.instance) GameManager.instance.ShowHint("HỆ THỐNG SẬP! KHỞI ĐỘNG LẠI CẦU DAO.");

        
        if (mainBreaker != null)
        {
            mainBreaker.ToggleMarker(true);
        }
    }

    public void RestorePower()
    {
        if (!isPowerOff) return;
        isPowerOff = false;

        if (audioSource && powerUpSFX) audioSource.PlayOneShot(powerUpSFX);
        foreach (var l in mapLights) if (l != null) l.enabled = true;

        if (mainBreaker != null)
        {
            mainBreaker.ToggleMarker(false);
        }

        // --- RESET LẠI THỜI GIAN ---
        // Từ lần thứ 2 trở đi, set thời gian là 3 phút (180s)
        currentTime = normalCycleDuration;

        UpdateTimerUI();
    }
}