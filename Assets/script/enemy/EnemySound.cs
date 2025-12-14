using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [Header("Cài đặt chung")]
    [SerializeField] AudioSource audioSource;

    [Header("Âm thanh Phát hiện")]
    [SerializeField] AudioClip alertSound;

    [Header("Âm thanh Tấn công")]
    [SerializeField] AudioClip attackSound; 

    
    private float lastAlertTime = -100f; // Mốc thời gian lần cuối kêu
    [SerializeField] float alertCooldown = 5f;     // Cứ 5 giây mới được kêu 1 lần

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnFootstep(AnimationEvent animationEvent)
    {
        
    }

    public void PlayAlertSound()
    {
        if (alertSound == null || audioSource == null) return;

        if (Time.time < lastAlertTime + alertCooldown) return;

        // Cập nhật lại thời gian vừa kêu
        lastAlertTime = Time.time;

        // Reset pitch và phát âm thanh
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(alertSound, 1f);
    }
    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
}