using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

// 1. Tạo class nhỏ để chứa cả Chữ và File ghi âm
[System.Serializable]
public class DialogueData
{
    [TextArea] public string lineText; // Nội dung câu thoại
    public AudioClip voiceClip;        // File âm thanh giọng nói
}

public class CheckPlayer : MonoBehaviour
{
    [Header("--- Cài đặt Camera Cinematic ---")]
    public CinemachineVirtualCamera bossVirtualCamera;

    [Header("--- Cài đặt UI Thoại ---")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    // 2. Thay đổi từ string[] sang mảng DialogueData mới
    public DialogueData[] dialogueContent;
    public GameObject skipButton; // Kéo nút Skip vào đây

    [Header("--- Cài đặt Âm thanh ---")]
    public AudioSource audioSource; // Kéo AudioSource vào để phát tiếng

    [Header("--- Cài đặt Tốc độ Chữ ---")]
    public float typingSpeed = 0.04f;

    [Header("--- Cài đặt Boss & Player ---")]
    public MonoBehaviour bossSkillScript;
    public MonoBehaviour playerMovement;

    private bool hasPlayed = false;
    private bool isSkipping = false; // Biến kiểm tra xem có đang skip không

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && (other.CompareTag("Player") || other.gameObject.name == "PlayerArmature"))
        {
            StartCoroutine(PlayCutscene());
        }
    }

    // Hàm này gán vào nút Button Skip trên màn hình (OnClick)
    public void SkipCutscene()
    {
        if (hasPlayed && !isSkipping)
        {
            isSkipping = true; // Bật cờ skip để thoát vòng lặp
        }
    }

    IEnumerator PlayCutscene()
    {
        hasPlayed = true;
        isSkipping = false;

        // --- CODE MỚI THÊM VÀO ĐÂY ---
        // Mở khóa chuột để người chơi có thể bấm nút Skip hoặc click màn hình
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Khóa Player và Boss
        if (playerMovement != null) playerMovement.enabled = false;
        if (bossSkillScript != null) bossSkillScript.enabled = false;

        // Chuyển Camera
        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 20;

        // Bật nút Skip (nếu có)
        if (skipButton != null) skipButton.SetActive(true);

        yield return new WaitForSeconds(1.5f); // Chờ camera bay

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // --- BẮT ĐẦU VÒNG LẶP THOẠI MỚI ---
        foreach (var data in dialogueContent)
        {
            // Nếu bấm Skip thì thoát vòng lặp ngay lập tức
            if (isSkipping) break;

            dialogueText.text = "";

            // A. PHÁT LỒNG TIẾNG
            if (audioSource != null && data.voiceClip != null)
            {
                audioSource.Stop(); // Dừng câu cũ
                audioSource.PlayOneShot(data.voiceClip); // Phát câu mới
            }

            // B. HIỆU ỨNG CHỮ CHẠY
            foreach (char letter in data.lineText.ToCharArray())
            {
                // Nếu bấm Skip trong lúc chữ đang chạy -> thoát luôn
                if (isSkipping) break;

                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Nếu bấm Skip -> thoát luôn
            if (isSkipping) break;

            // C. CHỜ NGƯỜI CHƠI CLICK CHUỘT TRÁI (Thay cho WaitForSeconds cũ)
            // Code sẽ kẹt ở dòng dưới mãi cho đến khi Click chuột HOẶC bấm Skip
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || isSkipping);
        }

        // --- KẾT THÚC CUTSCENE ---
        EndCutscene();
    }

    void EndCutscene()
    {
        // Tắt hết UI
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false);

        // --- CODE MỚI THÊM VÀO ĐÂY ---
        // Khóa chuột lại để quay về mode chơi game bắn súng
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // -----------------------------

        // Tắt âm thanh nếu đang nói dở
        if (audioSource != null) audioSource.Stop();

        // Trả Camera về
        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 0;

        // Mở khóa Player và kích hoạt Boss (Logic cũ của bạn)
        StartCoroutine(EnableGameplayDelayed());
    }

    IEnumerator EnableGameplayDelayed()
    {
        // Delay nhỏ để camera kịp quay về (nếu muốn mượt hơn)
        yield return new WaitForSeconds(1.0f);

        if (playerMovement != null) playerMovement.enabled = true;

        if (bossSkillScript != null)
        {
            bossSkillScript.enabled = true;
            // Giữ lại logic gọi hàm StartFighting của bạn
            var bossScript = bossSkillScript.GetComponent("BossLaserSkill") as MonoBehaviour;
            // Lưu ý: Mình dùng SendMessage hoặc GetComponent theo cách an toàn để tránh lỗi nếu bạn đổi tên script
            bossSkillScript.SendMessage("StartFighting", SendMessageOptions.DontRequireReceiver);
        }
    }
}