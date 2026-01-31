using System.Collections;
using UnityEngine;

public class BossLaserSkill : MonoBehaviour
{
    [Header("--- Cài đặt Prefab ---")]
    public GameObject warningZonePrefab;
    public GameObject laserPrefab;

    [Header("--- Cài đặt Âm Thanh [MỚI] ---")]
    public AudioSource audioSource;       // Kéo cái Loa vào đây
    public AudioClip warningSound;        // Tiếng "ting" cảnh báo
    public AudioClip laserSound;          // Tiếng "bùm" khi bắn
    [Range(0f, 1f)] public float soundVolume = 0.3f; // Chỉnh nhỏ thôi vì 10 tia kêu cùng lúc sẽ rất to

    [Header("--- Cài đặt Thời Gian ---")]
    public float skillActiveDuration = 10.0f; // Tổng thời gian skill hoạt động
    public float waveInterval = 2.0f;         // Cứ 2s bắn 1 đợt
    public float skillCooldown = 20.0f;       // Hồi chiêu sau khi xong hết

    [Header("--- Cài đặt Chi tiết đợt bắn ---")]
    public int lasersPerWave = 10;            // Số lượng tia mỗi đợt
    public float warningDuration = 1.5f;      // Thời gian cảnh báo
    public float laserLifeTime = 1.0f;        // Laze tồn tại bao lâu

    [Header("--- Cài đặt Khu Vực ---")]
    public float warningSize = 1.5f;
    public float laserSpawnHeight = 10.0f;
    public float warningHeight = 0.5f;

    [Header("--- Phạm Vi Ngẫu Nhiên (Map) ---")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    private Vector3 initialWarningScale;

    void Start()
    {
        // Tự động lấy AudioSource nếu quên kéo, nhưng tốt nhất bạn nên kéo tay
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (warningZonePrefab != null)
        {
            initialWarningScale = warningZonePrefab.transform.localScale;
        }

        Debug.Log("[BossSkill] Boss đang ngủ, chờ Player gọi...");
    }

    public void StartFighting()
    {
        Debug.Log("[BossSkill] Đã nhận lệnh chiến đấu!");
        StartCoroutine(AutoSkillLoop());
    }

    // Vòng lặp chính quản lý trạng thái Boss
    IEnumerator AutoSkillLoop()
    {
        while (true)
        {
            Debug.Log("[BossSkill] Đang chờ cooldown 25s sau hội thoại...");
            yield return new WaitForSeconds(25.0f);

            Debug.Log("[BossSkill] --- BẮT ĐẦU TRẠNG THÁI CUỒNG NỘ (10s) ---");

            float activeTimer = 0f;

            while (activeTimer < skillActiveDuration)
            {
                SpawnMultiShotWave();
                yield return new WaitForSeconds(waveInterval);
                activeTimer += waveInterval;
            }

            Debug.Log($"[BossSkill] Hết thời gian cuồng nộ. Đang hồi chiêu... ({skillCooldown}s)");
            yield return new WaitForSeconds(skillCooldown);
        }
    }

    // Hàm sinh ra 1 đợt gồm nhiều tia
    void SpawnMultiShotWave()
    {
        for (int i = 0; i < lasersPerWave; i++)
        {
            float rX = Random.Range(minX, maxX);
            float rZ = Random.Range(minZ, maxZ);
            Vector3 targetPos = new Vector3(rX, warningHeight, rZ);

            StartCoroutine(ProcessSingleStrike(targetPos));
        }
    }

    // Quy trình xử lý cho MỘT tia laze (CÓ THÊM ÂM THANH)
    IEnumerator ProcessSingleStrike(Vector3 position)
    {
        // --- GIAI ĐOẠN 1: CẢNH BÁO ---

        // [MỚI] Phát tiếng cảnh báo ngay khi vòng đỏ xuất hiện
        if (audioSource != null && warningSound != null)
        {
            // Dùng PlayOneShot để các âm thanh đè lên nhau được (không bị ngắt quãng)
            audioSource.PlayOneShot(warningSound, soundVolume);
        }

        GameObject warnObj = Instantiate(warningZonePrefab, position, Quaternion.identity);
        warnObj.transform.localScale = new Vector3(0, initialWarningScale.y, 0);

        float timer = 0f;
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / warningDuration;
            float currentSize = Mathf.Lerp(0, warningSize, progress);
            warnObj.transform.localScale = new Vector3(currentSize, initialWarningScale.y, currentSize);
            yield return null;
        }

        warnObj.transform.localScale = new Vector3(warningSize, initialWarningScale.y, warningSize);
        yield return new WaitForSeconds(0.1f);
        Destroy(warnObj);

        // --- GIAI ĐOẠN 2: BẮN LAZE ---

        // [MỚI] Phát tiếng laze nổ ngay khi tia laze xuất hiện
        if (audioSource != null && laserSound != null)
        {
            audioSource.PlayOneShot(laserSound, soundVolume);
        }

        Vector3 spawnPos = position + Vector3.up * laserSpawnHeight;
        GameObject laserObj = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(laserLifeTime);
        Destroy(laserObj);
    }
}