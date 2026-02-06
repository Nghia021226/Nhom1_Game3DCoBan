using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldSkill : MonoBehaviour
{
    [Header("--- Cài đặt Boss & Khiên ---")]
    public Transform bossCenter;          // Tâm Boss (ngực/bụng)
    public GameObject shieldObject;       // Cục Sphere (Giáp)
    public float shieldMaxSize = 3.0f;    // Kích thước giáp
    public float scaleSpeed = 2.0f;       // Tốc độ hiện giáp

    [Header("--- Cài đặt Điểm Neo Tường ---")]
    public GameObject[] allWallAnchors;   // Các điểm trên tường
    public int anchorsToActivate = 3;     // Số lượng cần kích hoạt

    [Header("--- Cài đặt Dây (Đơn giản) ---")]
    public GameObject beam3DPrefab;       // Prefab dây (Particle System cũ)

    // --- Biến nội bộ ---
    private List<GameObject> activeBeams = new List<GameObject>();
    private int currentActiveAnchors = 0;
    private bool isShieldActive = false;
    private Collider shieldCollider;

    void Start()
    {
        // Ẩn giáp khi bắt đầu
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
            shieldObject.transform.localScale = Vector3.zero;
            shieldCollider = shieldObject.GetComponent<Collider>();
            if (shieldCollider) shieldCollider.enabled = false;
        }

        // Ẩn các điểm neo
        foreach (var anchor in allWallAnchors)
        {
            if (anchor != null) anchor.SetActive(false);
        }
    }

    public void ActivateShieldSkill()
    {
        if (!isShieldActive) StartCoroutine(ProcessShieldSkill());
    }

    public bool IsSkillFinished()
    {
        return !isShieldActive;
    }

    IEnumerator ProcessShieldSkill()
    {
        isShieldActive = true;

        // BƯỚC 1: Chọn ngẫu nhiên điểm neo trên tường
        List<GameObject> shuffledAnchors = new List<GameObject>(allWallAnchors);
        for (int i = 0; i < shuffledAnchors.Count; i++)
        {
            GameObject temp = shuffledAnchors[i];
            int randomIndex = Random.Range(i, shuffledAnchors.Count);
            shuffledAnchors[i] = shuffledAnchors[randomIndex];
            shuffledAnchors[randomIndex] = temp;
        }

        currentActiveAnchors = 0;
        int count = Mathf.Min(anchorsToActivate, shuffledAnchors.Count);

        // BƯỚC 2: Bật Neo và Tạo Dây
        for (int i = 0; i < count; i++)
        {
            GameObject anchor = shuffledAnchors[i];
            if (anchor != null)
            {
                anchor.SetActive(true);
                currentActiveAnchors++;

                // Gắn script máu cho neo
                var anchorScript = anchor.GetComponent<ShieldAnchor>();
                if (anchorScript == null) anchorScript = anchor.AddComponent<ShieldAnchor>();
                anchorScript.Setup(this);

                // Tạo dây nối (Code mới siêu gọn)
                StartCoroutine(SpawnSimpleBeam(anchor.transform));
            }
        }

        // BƯỚC 3: Phình to Giáp Boss
        if (shieldObject != null)
        {
            shieldObject.SetActive(true);
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime * scaleSpeed;
                shieldObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * shieldMaxSize, timer);
                yield return null;
            }
            if (shieldCollider) shieldCollider.enabled = true;
        }

        // BƯỚC 4: Chờ Player phá hết neo
        while (currentActiveAnchors > 0)
        {
            yield return null;
        }

        // BƯỚC 5: Kết thúc skill
        DeactivateShield();
    }

    // --- HÀM TẠO DÂY ĐÃ ĐƯỢC ĐƠN GIẢN HÓA ---
    IEnumerator SpawnSimpleBeam(Transform anchorTransform)
    {
        if (beam3DPrefab == null || bossCenter == null) yield break;

        // 1. Tạo dây tại vị trí tường
        GameObject beam = Instantiate(beam3DPrefab, anchorTransform.position, Quaternion.identity);
        activeBeams.Add(beam);

        // 2. Xoay dây nhìn về phía Boss ngay lập tức
        beam.transform.LookAt(bossCenter);

        // 3. Vòng lặp duy trì sự tồn tại của dây (Không scale, không chỉnh vị trí)
        while (isShieldActive && beam != null && anchorTransform != null)
        {
            // Nếu Boss có nhúc nhích nhẹ (idle animation), cập nhật hướng nhìn cho chuẩn
            beam.transform.LookAt(bossCenter);

            // Giữ nguyên vị trí tại neo
            beam.transform.position = anchorTransform.position;

            yield return null;
        }

        // 4. Hủy dây khi xong
        if (beam != null) Destroy(beam);
    }

    public void OnAnchorDestroyed()
    {
        currentActiveAnchors--;
    }

    void DeactivateShield()
    {
        StartCoroutine(CloseShieldRoutine());
    }

    IEnumerator CloseShieldRoutine()
    {
        if (shieldCollider) shieldCollider.enabled = false;

        // Xóa hết dây
        foreach (var beam in activeBeams)
        {
            if (beam != null) Destroy(beam);
        }
        activeBeams.Clear();

        // Thu nhỏ giáp rồi tắt
        if (shieldObject != null)
        {
            float startScale = shieldObject.transform.localScale.x;
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime * scaleSpeed;
                float s = Mathf.Lerp(startScale, 0, timer);
                shieldObject.transform.localScale = new Vector3(s, s, s);
                yield return null;
            }
            shieldObject.SetActive(false);
        }

        isShieldActive = false;
    }
}