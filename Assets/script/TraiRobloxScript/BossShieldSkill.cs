using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldSkill : MonoBehaviour
{
    [Header("--- Cài đặt Boss & Khiên ---")]
    public Transform bossCenter;          // Kéo cái tâm Boss vào (vị trí dây sẽ cắm vào)
    public GameObject shieldObject;       // Cái cục khiên tròn bao quanh Boss
    public float shieldMaxSize = 3.0f;
    public float scaleSpeed = 2.0f;

    [Header("--- Cài đặt Điểm Neo Tường (NGUỒN) ---")]
    public GameObject[] allWallAnchors;   // Kéo hết mấy cái cục tròn trên tường vào đây
    public int anchorsToActivate = 3;     // Số lượng dây muốn nối mỗi lần (VD: có 10 điểm nhưng chỉ nối 3 dây ngẫu nhiên)

    [Header("--- Cài đặt Dây 3D ---")]
    public GameObject beam3DPrefab;       // Kéo cái Prefab "SingleLine-LightSaber" vào đây
    public float beamGrowSpeed = 10.0f;   // Tốc độ dây dài ra

    // --- Biến nội bộ để quản lý ---
    private List<GameObject> activeBeams = new List<GameObject>();
    private int currentActiveAnchors = 0;
    private bool isShieldActive = false;
    private Collider shieldCollider;

    void Start()
    {
        // 1. Tắt khiên boss ban đầu
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
            shieldObject.transform.localScale = Vector3.zero;
            shieldCollider = shieldObject.GetComponent<Collider>();
            if (shieldCollider) shieldCollider.enabled = false;
        }

        // 2. Tắt hết các điểm neo trên tường và cài đặt script cho nó
        foreach (var anchor in allWallAnchors)
        {
            if (anchor != null)
            {
                anchor.SetActive(false);
                // Tự động gắn script ShieldAnchor nếu quên gắn tay
                var anchorScript = anchor.GetComponent<ShieldAnchor>();
                if (anchorScript == null) anchorScript = anchor.AddComponent<ShieldAnchor>();
                anchorScript.Setup(this);
            }
        }
    }

    // --- HÀM NÀY ĐƯỢC GỌI TỪ BOSS CONTROLLER ---
    public void ActivateShieldSkill()
    {
        if (!isShieldActive)
        {
            StartCoroutine(ProcessShieldSkill());
        }
    }

    IEnumerator ProcessShieldSkill()
    {
        isShieldActive = true;

        // --- BƯỚC 1: RANDOM NGUỒN (Quan trọng) ---
        // Tạo một danh sách tạm để xáo trộn vị trí các điểm neo
        List<GameObject> shuffledAnchors = new List<GameObject>(allWallAnchors);

        // Thuật toán xáo trộn (Fisher-Yates Shuffle) để đảm bảo ngẫu nhiên thật sự
        for (int i = 0; i < shuffledAnchors.Count; i++)
        {
            GameObject temp = shuffledAnchors[i];
            int randomIndex = Random.Range(i, shuffledAnchors.Count);
            shuffledAnchors[i] = shuffledAnchors[randomIndex];
            shuffledAnchors[randomIndex] = temp;
        }

        // --- BƯỚC 2: KÍCH HOẠT CÁC ĐIỂM ĐÃ CHỌN ---
        currentActiveAnchors = 0;

        // Chỉ lấy số lượng điểm cần thiết (anchorsToActivate) từ danh sách đã xáo trộn
        int count = Mathf.Min(anchorsToActivate, shuffledAnchors.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject anchor = shuffledAnchors[i];

            // Bật cục tròn trên tường lên
            anchor.SetActive(true);
            currentActiveAnchors++;

            // Tạo dây nối từ Anchor đó tới Boss
            StartCoroutine(Spawn3DBeam(anchor.transform.position));
        }

        // --- BƯỚC 3: BẬT KHIÊN BOSS ---
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

        // --- BƯỚC 4: TREO SKILL CHO ĐẾN KHI BỊ PHÁ HẾT ---
        while (currentActiveAnchors > 0)
        {
            yield return null; // Chờ Player phá các điểm neo
        }

        // --- BƯỚC 5: KẾT THÚC ---
        DeactivateShield();
    }

    // --- HÀM TẠO DÂY 3D (ĐÃ UPDATE CHO OBJECT 3D) ---
    IEnumerator Spawn3DBeam(Vector3 startPos)
    {
        if (beam3DPrefab == null || bossCenter == null) yield break;

        // 1. Sinh ra dây tại vị trí tường (Start Pos)
        GameObject beam = Instantiate(beam3DPrefab, startPos, Quaternion.identity);
        activeBeams.Add(beam);

        // 2. Xoay dây hướng về phía Boss
        beam.transform.LookAt(bossCenter);

        // 3. Tính toán khoảng cách
        float targetDistance = Vector3.Distance(startPos, bossCenter.position);

        // Lưu lại scale gốc (để giữ độ dày X, Y của tia)
        Vector3 initialScale = beam.transform.localScale;

        // 4. Hiệu ứng dây dài ra từ từ
        float currentLength = 0f;
        while (currentLength < targetDistance)
        {
            // Nếu dây bị destroy hoặc skill kết thúc thì dừng lại
            if (beam == null || !isShieldActive) break;

            currentLength += Time.deltaTime * beamGrowSpeed;

            // Giới hạn không cho dài quá khoảng cách tới boss
            if (currentLength > targetDistance) currentLength = targetDistance;

            // --- QUAN TRỌNG: UPDATE SCALE ---
            // Giả sử trục Z của prefab là chiều dài (thường các asset LightSaber là vậy)
            // Nếu asset của bạn dùng trục Y là chiều dài thì sửa Z thành Y ở dòng dưới
            beam.transform.localScale = new Vector3(initialScale.x, initialScale.y, currentLength);

            // Liên tục cập nhật hướng (đề phòng boss di chuyển nhẹ)
            beam.transform.LookAt(bossCenter);

            yield return null;
        }
    }

    // Hàm gọi từ Anchor khi bị bắn
    public void OnAnchorDestroyed()
    {
        currentActiveAnchors--;

        // Có thể thêm hiệu ứng âm thanh vỡ kết nối ở đây
        Debug.Log("Đã phá 1 nguồn năng lượng!");
    }

    void DeactivateShield()
    {
        StartCoroutine(CloseShieldRoutine());
    }

    IEnumerator CloseShieldRoutine()
    {
        if (shieldCollider) shieldCollider.enabled = false;

        // Xóa hết các dây
        foreach (var beam in activeBeams)
        {
            if (beam != null) Destroy(beam);
        }
        activeBeams.Clear();

        // Thu nhỏ khiên
        if (shieldObject != null)
        {
            float currentScale = shieldObject.transform.localScale.x;
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime * scaleSpeed;
                float s = Mathf.Lerp(currentScale, 0, timer);
                shieldObject.transform.localScale = new Vector3(s, s, s);
                yield return null;
            }
            shieldObject.SetActive(false);
        }

        isShieldActive = false;
    }

    public bool IsSkillFinished()
    {
        return !isShieldActive;
    }
}