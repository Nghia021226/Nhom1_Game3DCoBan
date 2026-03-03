using UnityEngine;

public class AdvancedEnemySound : MonoBehaviour
{
    [Header("--- CẤU HÌNH LOA ---")]
    [Tooltip("Kéo AudioSource chính vào đây (để phát tiếng đánh/ăn)")]
    [SerializeField] private AudioSource sfxSource;

   
    private AudioSource ambientSource;

    [Header("--- ÂM THANH MÔI TRƯỜNG (Gầm gừ) ---")]
    public AudioClip idleGrowlClip;
    [Range(0f, 1f)] public float growlVolume = 0.6f;

    [Header("--- ÂM THANH CHIÊU THỨC ---")]
    public AudioClip jumpAttackClip; 
    public AudioClip biteAttackClip;
    public AudioClip handAttackClip; 
    public AudioClip eatClip;        

    void Start()
    {
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        SetupAmbientAudio();
    }

    void SetupAmbientAudio()
    {
        if (idleGrowlClip == null) return;

        GameObject g = new GameObject("Ambient_Audio_Source");
        g.transform.SetParent(this.transform);
        g.transform.localPosition = Vector3.zero;

        ambientSource = g.AddComponent<AudioSource>();
        ambientSource.clip = idleGrowlClip;
        ambientSource.loop = true;
        ambientSource.volume = growlVolume;
        ambientSource.spatialBlend = 1f;
        ambientSource.minDistance = 2f;
        ambientSource.maxDistance = 15f; 
        ambientSource.rolloffMode = AudioRolloffMode.Linear;
        ambientSource.dopplerLevel = 0f;

        ambientSource.Play();
    }

    
    public void ToggleGrowl(bool isEnable)
    {
        if (ambientSource == null) return;

        if (isEnable)
        {
            if (!ambientSource.isPlaying) ambientSource.Play();
        }
        else
        {
            if (ambientSource.isPlaying) ambientSource.Pause();
        }
    }

    public void PlayAttackSound(string type)
    {
        if (sfxSource == null) return;

        AudioClip clipToPlay = null;
        float pitchRandom = Random.Range(0.9f, 1.1f);

        switch (type)
        {
            case "Jump":
                clipToPlay = jumpAttackClip;
                break;
            case "Bite":
                clipToPlay = biteAttackClip;
                break;
            case "Hand":
                clipToPlay = handAttackClip;
                break;
            case "Eat":
                clipToPlay = eatClip;
                pitchRandom = Random.Range(1.0f, 1.2f);
                break;
        }

        if (clipToPlay != null)
        {
            sfxSource.pitch = pitchRandom;
           
            sfxSource.PlayOneShot(clipToPlay);
        }
    }
}