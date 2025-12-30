using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets; // Để khóa chân nhân vật

public class CCTVController : MonoBehaviour
{
    [Header("Cài đặt Camera")]
    [SerializeField] Camera securityCamera; // Kéo cái SecurityCamera vào đây
    [SerializeField] Transform[] camPositions; // Kéo các CamPos_1, CamPos_2... vào đây
    [SerializeField] string[] camNames; // Đặt tên cho từng cam: "Cam 1", "Hành lang"...

    [Header("Cài đặt UI")]
    [SerializeField] GameObject cctvPanel; // Kéo cái CCTV_Panel vào
    [SerializeField] TextMeshProUGUI camNameText; // Kéo cái Text hiển thị tên Cam
    [SerializeField] GameObject noiseEffect; // (Tùy chọn) Hiệu ứng nhiễu hạt

    [Header("Kết nối Player")]
    [SerializeField] GameObject player; // Kéo nhân vật vào
    private StarterAssetsInputs _input; // Để chặn di chuyển

    private bool isUsingCCTV = false;
    private int currentCamIndex = 0;

    void Start()
    {
        // Ẩn hệ thống lúc đầu
        if (cctvPanel) cctvPanel.SetActive(false);
        if (securityCamera) securityCamera.gameObject.SetActive(false);

        if (player != null)
            _input = player.GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        // Bấm J để Bật/Tắt
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleCCTV();
        }

        // Nếu đang xem Cam thì bấm mũi tên để chuyển
        if (isUsingCCTV)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                ChangeCamera(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                ChangeCamera(-1);
            }
        }
    }

    void ToggleCCTV()
    {
        isUsingCCTV = !isUsingCCTV;

        if (isUsingCCTV)
        {
            // --- BẬT CAMERA ---
            cctvPanel.SetActive(true);
            securityCamera.gameObject.SetActive(true);

            // Khóa chân nhân vật
            if (_input != null)
            {
                _input.cursorInputForLook = false; // Ngừng xoay chuột
                _input.move = Vector2.zero; // Ngừng đi
            }

            // Hiện chuột (để có thể làm gì đó trên UI nếu cần) hoặc ẩn tùy bạn
            // Ở đây mình cứ giữ nguyên trạng thái chuột của game

            // Cập nhật cam đầu tiên
            UpdateCameraView();

            // Ẩn gợi ý nếu có
            if (GameManager.instance != null) GameManager.instance.HideHint();
        }
        else
        {
            // --- TẮT CAMERA ---
            cctvPanel.SetActive(false);
            securityCamera.gameObject.SetActive(false);

            // Mở lại nhân vật
            if (_input != null)
            {
                _input.cursorInputForLook = true;
            }
        }
    }

    void ChangeCamera(int direction)
    {
        currentCamIndex += direction;

        // Xử lý vòng lặp (Đang ở cam cuối bấm tiếp thì về cam đầu)
        if (currentCamIndex >= camPositions.Length) currentCamIndex = 0;
        if (currentCamIndex < 0) currentCamIndex = camPositions.Length - 1;

        UpdateCameraView();
    }

    void UpdateCameraView()
    {
        if (camPositions.Length == 0) return;

        // 1. Dịch chuyển Camera thật tới vị trí của CamPos ảo
        Transform targetPos = camPositions[currentCamIndex];
        securityCamera.transform.position = targetPos.position;
        securityCamera.transform.rotation = targetPos.rotation;

        // 2. Cập nhật tên
        if (camNameText != null && camNames.Length > currentCamIndex)
        {
            camNameText.text = camNames[currentCamIndex];
        }

        // 3. (Optional) Hiệu ứng nháy nhiễu mỗi khi chuyển cam cho thật
        // Bạn có thể thêm code play sound "bip" ở đây
    }
}