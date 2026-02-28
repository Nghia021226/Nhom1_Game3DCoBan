using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;
[System.Serializable]
public class DialogueData
{
    [TextArea] public string lineText; 
    public AudioClip voiceClip;        
}

public class CheckPlayer : MonoBehaviour
{
    [Header("--- Cài đặt Camera Cinematic ---")]
    [SerializeField] CinemachineVirtualCamera bossVirtualCamera;

    [Header("--- Cài đặt UI Thoại ---")]
    private GameObject dialoguePanel;
    private TextMeshProUGUI dialogueText;

    private DialogueData[] dialogueContent;
    [SerializeField] GameObject skipButton;

    [Header("--- Cài đặt Âm thanh ---")]
    private AudioSource audioSource;
    private AudioSource bgmAudioSource;

    [Header("--- Cài đặt Tốc độ Chữ ---")]
    [SerializeField] float typingSpeed = 0.04f;

    [Header("--- Cài đặt Boss & Player ---")]
    [SerializeField] MonoBehaviour bossSkillScript;
    [SerializeField] MonoBehaviour playerMovement;
    [SerializeField] BossHealth bossHealthScript;
    private bool hasPlayed = false;
    private bool isSkipping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && (other.CompareTag("Player") || other.gameObject.name == "PlayerArmature"))
        {
            StartCoroutine(PlayCutscene());
        }
    }

    public void SkipCutscene()
    {
        if (hasPlayed && !isSkipping)
        {
            isSkipping = true;
        }
    }

    IEnumerator PlayCutscene()
    {
        hasPlayed = true;
        isSkipping = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerMovement != null) playerMovement.enabled = false;
        if (bossSkillScript != null) bossSkillScript.enabled = false;

        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 20;

        if (skipButton != null) skipButton.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        foreach (var data in dialogueContent)
        {
            if (isSkipping) break;

            dialogueText.text = "";

            if (audioSource != null && data.voiceClip != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(data.voiceClip);
            }

            foreach (char letter in data.lineText.ToCharArray())
            {
                if (isSkipping) break;

                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            if (isSkipping) break;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || isSkipping);
        }

        EndCutscene();
    }

    void EndCutscene()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (skipButton != null) skipButton.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (audioSource != null) audioSource.Stop();

        if (bossVirtualCamera != null) bossVirtualCamera.Priority = 0;

        StartCoroutine(EnableGameplayDelayed());
    }

    IEnumerator EnableGameplayDelayed()
    {
        Debug.Log("[CheckPlayer] Bắt đầu kích hoạt lại gameplay sau cutscene...");
        yield return new WaitForSeconds(1.0f);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (bossSkillScript != null)
        {
            bossSkillScript.enabled = true;
            bossSkillScript.SendMessage("StartFighting", SendMessageOptions.DontRequireReceiver);
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
            Debug.Log("[CheckPlayer] ĐÃ BẬT NHẠC NỀN BOSS!");
        }

        if (bossHealthScript != null)
        {
            bossHealthScript.gameObject.SetActive(true);
            bossHealthScript.StartFighting();
        }
        else
        {
            Debug.LogError("🚨 [CheckPlayer] LỖI CỰC MẠNH: bossHealthScript đang bị NULL!");
        }
    }
    public void StopBossMusic()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("[CheckPlayer] ĐÃ TẮT NHẠC NỀN DO BOSS CHẾT!");
        }
    }
}