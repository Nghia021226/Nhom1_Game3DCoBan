using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySoundController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip hitSound;
    public AudioClip dieSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float hitVolume = 1f;

    [Range(0f, 1f)]
    public float dieVolume = 1f;

    private AudioSource audioSource;
    private bool isDead;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
    }

    // ===== ANIMATION EVENT =====
    // Gọi ở animation Hit
    public void PlayHitSound()
    {
        if (isDead) return;
        if (hitSound == null) return;

        audioSource.PlayOneShot(hitSound, hitVolume);
    }

    // ===== ANIMATION EVENT =====
    // Gọi ở animation Die
    public void PlayDieSound()
    {
        if (isDead) return;
        isDead = true;

        if (dieSound == null) return;

        audioSource.PlayOneShot(dieSound, dieVolume);
    }
}
