using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Thêm thư viện này để dùng HashSet

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Cài đặt Nhạc")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip normalMusic;
    [SerializeField] AudioClip chaseMusic;

    // Thay đổi từ int sang HashSet để quản lý chính xác từng GameObject
    private HashSet<GameObject> chasingEnemies = new HashSet<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SwitchMusic(normalMusic);
    }

    // Nhận thêm tham số GameObject để biết chính xác quái nào đang đuổi
    public void StartChase(GameObject enemy)
    {
        if (chasingEnemies.Add(enemy)) // Chỉ thực hiện nếu quái này chưa có trong danh sách
        {
            if (chasingEnemies.Count == 1)
            {
                SwitchMusic(chaseMusic);
            }
        }
    }

    // Xóa quái khỏi danh sách khi nó ngừng đuổi hoặc chết
    public void StopChase(GameObject enemy)
    {
        if (chasingEnemies.Remove(enemy)) // Chỉ thực hiện nếu quái này có trong danh sách
        {
            if (chasingEnemies.Count == 0)
            {
                SwitchMusic(normalMusic);
            }
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