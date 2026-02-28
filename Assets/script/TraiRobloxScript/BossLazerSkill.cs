using System.Collections;
using UnityEngine;

public class BossLaserSkill : MonoBehaviour
{
    [Header("--- KẾT NỐI COMPONENT ---")]
    [SerializeField] private BossShieldSkill shieldSkillScript;

    [Header("--- PREFAB (VẬT THỂ) ---")]
    [SerializeField] private GameObject warningZonePrefab;
    [SerializeField] private GameObject laserPrefab;

    [Header("--- CHỈ SỐ KỸ NĂNG: MƯA LAZE ---")]
    [Tooltip("Số đợt bắn laze trong 1 lần Boss dùng chiêu này.")]
    [SerializeField] private int wavesPerSkill = 15;
    [Tooltip("Số lượng tia laze rơi xuống ngẫu nhiên trong MỖI đợt.")]
    [SerializeField] private int lasersPerWave = 10;
    [Tooltip("Thời gian chờ giữa các đợt bắn laze (giây).")]
    [SerializeField] private float waveInterval = 2.0f;

    [Header("--- CHỈ SỐ THỜI GIAN & HÌNH ẢNH ---")]
    [Tooltip("Thời gian chờ từ lúc vòng cảnh báo hiện lên đến lúc laze thực sự rơi xuống (giây).")]
    [SerializeField] private float warningDuration = 1.5f;
    [Tooltip("Thời gian tia laze tồn tại trên màn hình trước khi biến mất (giây).")]
    [SerializeField] private float laserLifeTime = 1.0f;
    [Tooltip("Kích thước nở ra tối đa của vòng tròn cảnh báo.")]
    [SerializeField] private float warningSize = 1.5f;

    [Header("--- TỌA ĐỘ & VỊ TRÍ ---")]
    [Tooltip("Chiều cao trên không trung mà tia laze sẽ xuất hiện để bắn thẳng xuống.")]
    [SerializeField] private float laserSpawnHeight = 10.0f;
    [Tooltip("Độ cao của vòng tròn cảnh báo nhô lên so với mặt đất (để tránh bị kẹt hình vào sàn).")]
    [SerializeField] private float warningHeight = 0.5f;

    [Space(10)] // Tạo một khoảng trống nhỏ cho dễ nhìn
    [Tooltip("Giới hạn bản đồ trục X (tối thiểu) để laze rơi ngẫu nhiên.")]
    [SerializeField] private float minX = -10f;
    [Tooltip("Giới hạn bản đồ trục X (tối đa).")]
    [SerializeField] private float maxX = 10f;
    [Tooltip("Giới hạn bản đồ trục Z (tối thiểu) để laze rơi ngẫu nhiên.")]
    [SerializeField] private float minZ = -10f;
    [Tooltip("Giới hạn bản đồ trục Z (tối đa).")]
    [SerializeField] private float maxZ = 10f;

    [Header("--- THỜI GIAN NGHỈ NHỊP CHUYỂN CHIÊU ---")]
    [Tooltip("Thời gian Boss đứng nghỉ ngơi sau khi xài xong 1 chiêu (giây).")]
    [SerializeField] private float timeBetweenSkills = 5.0f;

    [Header("--- ÂM THANH ---")]
    [Tooltip("Nguồn phát âm thanh gắn trên người Boss.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("File âm thanh tiếng 'bíp bíp' lúc cảnh báo đỏ.")]
    [SerializeField] private AudioClip warningSound;
    [Tooltip("File âm thanh tiếng laze chớp xuống.")]
    [SerializeField] private AudioClip laserSound;
    [Range(0f, 1f)]
    [Tooltip("Chỉnh âm lượng của các hiệu ứng âm thanh này (từ 0 đến 1).")]
    [SerializeField] private float soundVolume = 0.3f;

    private Vector3 initialWarningScale;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (warningZonePrefab != null) initialWarningScale = warningZonePrefab.transform.localScale;
    }
    public void StartFighting()
    {
        Debug.Log("[Boss] Đã nhận lệnh chiến đấu!");
        StartCoroutine(BossBattleLoop());
    }
    IEnumerator BossBattleLoop()
    {
        yield return new WaitForSeconds(5.0f);

        while (true)
        {
            int randomSkill = Random.Range(0, 2);

            if (randomSkill == 0)
            {
                Debug.Log("[Boss] Skill: MƯA LAZE!");
                yield return StartCoroutine(DoLaserSkill());
            }
            else
            {
                Debug.Log("[Boss] Skill: TẠO GIÁP!");
                if (shieldSkillScript != null)
                {
                    shieldSkillScript.ActivateShieldSkill();
                    yield return new WaitUntil(() => shieldSkillScript.IsSkillFinished());
                }
            }

            Debug.Log($"[Boss] Nghỉ mệt {timeBetweenSkills}s...");
            yield return new WaitForSeconds(timeBetweenSkills);
        }
    }
    IEnumerator DoLaserSkill()
    {
        for (int i = 0; i < wavesPerSkill; i++)
        {
            SpawnMultiShotWave();
            yield return new WaitForSeconds(waveInterval);
        }
    }
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
    IEnumerator ProcessSingleStrike(Vector3 position)
    {
        if (audioSource != null && warningSound != null) audioSource.PlayOneShot(warningSound, soundVolume);

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
        Destroy(warnObj);

        if (audioSource != null && laserSound != null) audioSource.PlayOneShot(laserSound, soundVolume);

        Vector3 spawnPos = position + Vector3.up * laserSpawnHeight;
        GameObject laserObj = Instantiate(laserPrefab, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(laserLifeTime);
        Destroy(laserObj);
    }
}