using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Cài đặt Nhạc")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip normalMusic;
    [SerializeField] AudioClip chaseMusic;

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

    public void StartChase(GameObject enemy)
    {
        if (chasingEnemies.Add(enemy)) 
        {
            if (chasingEnemies.Count == 1)
            {
                SwitchMusic(chaseMusic);
            }
        }
    }

    public void StopChase(GameObject enemy)
    {
        if (chasingEnemies.Remove(enemy)) 
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