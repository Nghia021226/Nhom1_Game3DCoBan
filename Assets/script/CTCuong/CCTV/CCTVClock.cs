using UnityEngine;
using TMPro;
using System; // Cần cái này để lấy giờ

public class CCTVClock : MonoBehaviour
{
    TextMeshProUGUI timeText;

    void Start()
    {
        timeText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Hiển thị dạng: 12/30/2025 08:22:10 PM
        // Bạn có thể chỉnh lại định dạng tùy thích
        string currentTime = DateTime.Now.ToString("MM/dd/yyyy   hh:mm:ss tt");

        // Nếu muốn thêm chữ giả lập
        timeText.text = "SYS_DATE: " + currentTime;
    }
}