using UnityEngine;

public class EnemySoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip hitSound;
    public AudioClip dieSound;

    public void PlayShoot()
    {
        audioSource.PlayOneShot(shootSound);
    }

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayDie()
    {
        audioSource.PlayOneShot(dieSound);
    }
}
