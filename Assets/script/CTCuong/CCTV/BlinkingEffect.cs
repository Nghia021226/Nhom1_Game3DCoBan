using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BlinkingEffect : MonoBehaviour
{
    [Header("Cài đặt")]
    [SerializeField] float speed = 0.7f;

    private TextMeshProUGUI textComp;
    private Image imageComp;

    // Dùng Awake để lấy component (chạy 1 lần duy nhất để tìm component)
    void Awake()
    {
        textComp = GetComponent<TextMeshProUGUI>();
        imageComp = GetComponent<Image>();
    }

    // Dùng OnEnable thay vì Start
    // Lợi ích: Mỗi lần bạn bấm J bật bảng CCTV lên, hàm này sẽ kích hoạt lại từ đầu
    void OnEnable()
    {
        StartCoroutine(BlinkRoutine());
    }

    // Hàm này chạy khi bảng CCTV bị tắt
    void OnDisable()
    {
        StopAllCoroutines(); // Dừng việc đếm giờ

        // Quan trọng: Bật hiện lại ngay lập tức để lần sau mở lên nó không bị tàng hình
        if (textComp) textComp.enabled = true;
        if (imageComp) imageComp.enabled = true;
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // --- TẮT (Ẩn đi) ---
            if (textComp) textComp.enabled = false;
            if (imageComp) imageComp.enabled = false;

            yield return new WaitForSeconds(speed / 2);

            // --- BẬT (Hiện lên) ---
            if (textComp) textComp.enabled = true;
            if (imageComp) imageComp.enabled = true;

            yield return new WaitForSeconds(speed);
        }
    }
}