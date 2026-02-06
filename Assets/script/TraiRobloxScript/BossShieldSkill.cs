using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldSkill : MonoBehaviour
{
    [Header("--- Cài đặt Boss & Khiên ---")]
    public Transform bossCenter;
    public GameObject shieldObject;
    public float shieldMaxSize = 3.0f;
    public float scaleSpeed = 2.0f;

    [Header("--- Cài đặt Điểm Neo Tường ---")]
    public GameObject[] allWallAnchors;
    public int anchorsToActivate = 3;

    [Header("--- Cài đặt Dây ---")]
    public GameObject beam3DPrefab;

    // ==========================================
    // PHẦN MỚI THÊM: CÀI ĐẶT ÂM THANH
    // ==========================================
    [Header("--- Cài đặt Âm thanh (MỚI) ---")]
    public AudioSource shieldAudioSource; // Kéo BossShieldObject (đã gắn AudioSource) vào đây
    public AudioClip shieldActivateSound; // Kéo file âm thanh "Bùm" khi bật khiên vào đây
    [Range(0f, 1f)] public float shieldVolume = 1.0f;

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

        // BƯỚC 1: Chọn ngẫu nhiên điểm neo
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

                var anchorScript = anchor.GetComponent<ShieldAnchor>();
                if (anchorScript == null) anchorScript = anchor.AddComponent<ShieldAnchor>();
                anchorScript.Setup(this);

                StartCoroutine(SpawnSimpleBeam(anchor.transform));
            }
        }

        // BƯỚC 3: Phình to Giáp Boss & PHÁT ÂM THANH
        if (shieldObject != null)
        {
            shieldObject.SetActive(true);

            // ---> PHÁT ÂM THANH TẠI ĐÂY <---
            if (shieldAudioSource != null && shieldActivateSound != null)
            {
                shieldAudioSource.PlayOneShot(shieldActivateSound, shieldVolume);
            }

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

    IEnumerator SpawnSimpleBeam(Transform anchorTransform)
    {
        if (beam3DPrefab == null || bossCenter == null) yield break;

        // Tạo dây (Âm thanh dây sẽ tự phát nhờ cài đặt trên Prefab - Xem hướng dẫn bên dưới)
        GameObject beam = Instantiate(beam3DPrefab, anchorTransform.position, Quaternion.identity);
        activeBeams.Add(beam);

        beam.transform.LookAt(bossCenter);

        while (isShieldActive && beam != null && anchorTransform != null)
        {
            beam.transform.LookAt(bossCenter);
            beam.transform.position = anchorTransform.position;
            yield return null;
        }

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

        foreach (var beam in activeBeams)
        {
            if (beam != null) Destroy(beam);
        }
        activeBeams.Clear();

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