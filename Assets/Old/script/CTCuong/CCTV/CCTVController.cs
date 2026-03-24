using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using System.Collections; // Cần thêm dòng này để dùng Coroutine

public class CCTVController : MonoBehaviour
{
    [Header("Cài đặt Camera")]
    [SerializeField] Camera securityCamera;
    [SerializeField] Transform[] camPositions;
    [SerializeField] string[] camNames;

    [Header("Cài đặt UI")]
    [SerializeField] GameObject cctvPanel;
    [SerializeField] TextMeshProUGUI camNameText;

    // --- PHẦN MỚI: HIỆU ỨNG & ÂM THANH ---
    [Header("Hiệu ứng CCTV")]
    [SerializeField] Image noiseOverlay;       // Kéo ảnh Noise vào đây (nhớ chỉnh Raycast Target = false)
    [SerializeField] AudioSource audioSource;  // Kéo AudioSource vào
    [SerializeField] AudioClip switchSound;    // Kéo file tiếng "Xoẹt" (Static)
    [Range(0f, 1f)][SerializeField] float normalNoiseAlpha = 0.15f; // Độ mờ nhiễu mặc định

    [Header("Kết nối Player")]
    [SerializeField] GameObject player;
    private StarterAssetsInputs _input;

    private bool isUsingCCTV = false;
    private int currentCamIndex = 0;

    void Start()
    {
        if (cctvPanel) cctvPanel.SetActive(false);
        if (securityCamera) securityCamera.gameObject.SetActive(false);

        if (player != null)
            _input = player.GetComponent<StarterAssetsInputs>();

        // Thiết lập độ mờ nhiễu ban đầu
        if (noiseOverlay != null)
            noiseOverlay.color = new Color(1, 1, 1, normalNoiseAlpha);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleCCTV();
        }

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
            // --- BẬT ---
            cctvPanel.SetActive(true);
            securityCamera.gameObject.SetActive(true);

            if (_input != null)
            {
                _input.cursorInputForLook = false;
                _input.move = Vector2.zero;
            }

            UpdateCameraView();
            PlaySwitchEffect(); // Phát tiếng khi bật lên luôn cho ngầu

            if (GameManager.instance != null) GameManager.instance.HideHint();
        }
        else
        {
            // --- TẮT ---
            cctvPanel.SetActive(false);
            securityCamera.gameObject.SetActive(false);

            if (_input != null)
            {
                _input.cursorInputForLook = true;
            }

            // Dừng tiếng rè nếu tắt cam
            if (audioSource != null) audioSource.Stop();
        }
    }

    void ChangeCamera(int direction)
    {
        currentCamIndex += direction;

        if (currentCamIndex >= camPositions.Length) currentCamIndex = 0;
        if (currentCamIndex < 0) currentCamIndex = camPositions.Length - 1;

        UpdateCameraView();
        PlaySwitchEffect(); // Hiệu ứng khi chuyển kênh
    }

    void PlaySwitchEffect()
    {
        // 1. Âm thanh
        if (audioSource != null && switchSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Đổi giọng tí cho đỡ nhàm
            audioSource.PlayOneShot(switchSound);
        }

        // 2. Hình ảnh (Nháy nhiễu)
        if (noiseOverlay != null && gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(GlitchRoutine());
        }
    }

    IEnumerator GlitchRoutine()
    {
        // Làm màn hình nhiễu trắng xóa trong tích tắc
        noiseOverlay.color = new Color(1, 1, 1, 0.8f);
        yield return new WaitForSeconds(0.1f);

        // Trả về bình thường
        noiseOverlay.color = new Color(1, 1, 1, normalNoiseAlpha);
    }

    void UpdateCameraView()
    {
        if (camPositions.Length == 0) return;

        Transform targetPos = camPositions[currentCamIndex];
        securityCamera.transform.position = targetPos.position;
        securityCamera.transform.rotation = targetPos.rotation;

        if (camNameText != null && camNames.Length > currentCamIndex)
        {
            camNameText.text = camNames[currentCamIndex];
        }
    }
}