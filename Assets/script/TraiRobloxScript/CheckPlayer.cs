using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

public class CheckPlayer : MonoBehaviour
{
    [Header("--- Cài đặt Camera Cinematic (Dùng Cinemachine) ---")]
    public CinemachineVirtualCamera bossVirtualCamera;

    [Header("--- Cài đặt UI Thoại ---")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] bossLines;

    [Header("--- Cài đặt Tốc độ Chữ (Typewriter) ---")]
    public float typingSpeed = 0.04f;  // Tốc độ gõ từng chữ
    public float waitAfterLine = 1.5f; // Thời gian chờ sau khi gõ xong 1 câu

    [Header("--- Cài đặt Boss & Player ---")]
    public MonoBehaviour bossSkillScript;
    public MonoBehaviour playerMovement;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && (other.CompareTag("Player") || other.gameObject.name == "PlayerArmature"))
        {
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        hasPlayed = true;

        // 1. Khóa Player và Boss
        if (playerMovement != null) playerMovement.enabled = false;
        if (bossSkillScript != null) bossSkillScript.enabled = false;

        // 2. Chuyển Camera
        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 20;

        // Chờ camera bay đến nơi
        yield return new WaitForSeconds(1.5f);

        // 3. Bật khung thoại
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // --- 4. HIỆU ỨNG CHỮ CHẠY (TYPEWRITER) ---
        foreach (string line in bossLines)
        {
            dialogueText.text = ""; // Reset khung chữ

            // Tách câu thành mảng ký tự và chạy từng cái
            foreach (char letter in line.ToCharArray())
            {
                dialogueText.text += letter; // Nối thêm chữ vào
                yield return new WaitForSeconds(typingSpeed); // Độ trễ giữa các chữ
            }

            // Đọc xong câu thì đợi xíu mới qua câu khác hoặc tắt
            yield return new WaitForSeconds(waitAfterLine);
        }
        // ------------------------------------------

        // 5. Kết thúc: Tắt thoại
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // Trả Camera về
        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 0;

        yield return new WaitForSeconds(1.0f);

        // 6. MỞ KHÓA VÀ KÍCH HOẠT BOSS
        if (playerMovement != null) playerMovement.enabled = true;

        if (bossSkillScript != null)
        {
            bossSkillScript.enabled = true;
            var bossScript = bossSkillScript.GetComponent<BossLaserSkill>();
            if (bossScript != null) bossScript.StartFighting();
        }
    }
}