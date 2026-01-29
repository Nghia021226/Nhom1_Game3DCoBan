using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Cài đặt Nhạc")]
    [SerializeField] AudioSource bgmSource;      
    [SerializeField] AudioClip normalMusic;      
    [SerializeField] AudioClip chaseMusic;       

    [Header("Thông số (Chỉ để xem)")]
    public int enemyChasingCount = 0;  

    void Awake()
    {
        // Tạo Singleton để gọi từ bất kỳ đâu
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        
        SwitchMusic(normalMusic);
    }

    // Hàm này gọi khi Quái bắt đầu đuổi
    public void StartChase()
    {
        enemyChasingCount++;

        // Nếu đây là con quái ĐẦU TIÊN phát hiện -> Đổi nhạc Chase ngay
        if (enemyChasingCount == 1)
        {
            SwitchMusic(chaseMusic);
        }
    }

    // Hàm này gọi khi Quái bỏ cuộc 
    public void StopChase()
    {
        enemyChasingCount--;

        // Giữ số không bao giờ âm
        if (enemyChasingCount < 0) enemyChasingCount = 0;

        
        if (enemyChasingCount == 0)
        {
            SwitchMusic(normalMusic);
        }
    }

    
    void SwitchMusic(AudioClip newClip)
    {
        if (bgmSource == null) return;

        if (newClip == null)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
            return;
        }

        if (bgmSource.clip == newClip && bgmSource.isPlaying) return;

        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}