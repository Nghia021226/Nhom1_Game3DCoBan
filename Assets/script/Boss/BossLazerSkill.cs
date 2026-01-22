using System.Collections;
using UnityEngine;

public class BossLaserSkill : MonoBehaviour
{
    [Header("--- Cài đặt Prefab ---")]
    public GameObject warningZonePrefab;
    public GameObject laserPrefab;

    [Header("--- Cài đặt Tấn Công ---")]
    public float warningDuration = 2.0f;
    public float maxWarningSize = 5.0f;
    public float laserDuration = 1.0f;
    public float laserSpawnHeight = 10.0f;

    [Tooltip("Độ cao của vòng tròn đỏ so với mặt đất (Chỉnh cao lên nếu bị khuất)")]
    public float warningHeight = 0.5f; // <--- MỚI THÊM: Mặc định là 0.5

    [Header("--- Mục tiêu ---")]
    public Transform playerTarget;

    private Vector3 initialWarningScale;

    void Start()
    {
        if (warningZonePrefab != null)
        {
            initialWarningScale = warningZonePrefab.transform.localScale;
        }
    }

    void Update()
    {
        // Phím T để test
        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 targetPos = playerTarget != null ? playerTarget.position : Vector3.zero;
            CastSkill(targetPos);
        }
    }

    public void CastSkill(Vector3 targetPosition)
    {
        // --- SỬA Ở ĐÂY: Dùng biến warningHeight thay vì số cứng 0.05f ---
        Vector3 groundPosition = new Vector3(targetPosition.x, warningHeight, targetPosition.z);

        StartCoroutine(ExecuteAttackSequence(groundPosition));
    }

    IEnumerator ExecuteAttackSequence(Vector3 position)
    {
        // Tạo vùng cảnh báo
        GameObject currentWarning = Instantiate(warningZonePrefab, position, Quaternion.identity);
        currentWarning.transform.localScale = new Vector3(0, initialWarningScale.y, 0);

        float timer = 0f;

        // Vòng lặp to dần
        while (timer < warningDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / warningDuration;
            float currentSize = Mathf.Lerp(0, maxWarningSize, progress);
            currentWarning.transform.localScale = new Vector3(currentSize, initialWarningScale.y, currentSize);
            yield return null;
        }

        currentWarning.transform.localScale = new Vector3(maxWarningSize, initialWarningScale.y, maxWarningSize);
        yield return new WaitForSeconds(0.1f);

        // Xóa cảnh báo và bắn Laze
        Destroy(currentWarning);

        Vector3 laserSpawnPos = position + Vector3.up * laserSpawnHeight;
        GameObject currentLaser = Instantiate(laserPrefab, laserSpawnPos, Quaternion.Euler(0, 0, 0));

        yield return new WaitForSeconds(laserDuration);
        Destroy(currentLaser);
    }
}