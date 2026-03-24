using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldSkill : MonoBehaviour
{
    [Header("--- Cài đặt Boss & Khiên ---")]
    [SerializeField] Transform bossCenter;
    [SerializeField] GameObject shieldObject;
    [SerializeField] float shieldMaxSize = 3.0f;
    [SerializeField] float scaleSpeed = 2.0f;

    [Header("--- Cài đặt Điểm Neo Tường ---")]
    public GameObject[] allWallAnchors;
    [SerializeField] int anchorsToActivate = 3;

    [Header("--- Cài đặt Dây ---")]
    [SerializeField] GameObject beam3DPrefab;

    [Header("--- Cài đặt Âm thanh ---")]
    [SerializeField] AudioSource shieldAudioSource;
    [SerializeField] AudioClip shieldActivateSound;
    [Range(0f, 1f)] public float shieldVolume = 1.0f;

    private List<GameObject> activeBeams = new List<GameObject>();
    private int currentActiveAnchors = 0;
    private bool isShieldActive = false;
    private Collider shieldCollider;

    void Start()
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
            shieldObject.transform.localScale = Vector3.zero;
            shieldCollider = shieldObject.GetComponent<Collider>();
            if (shieldCollider) shieldCollider.enabled = false;
        }

        foreach (GameObject anchor in allWallAnchors)
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

        for (int i = 0; i < count; i++)
        {
            GameObject anchor = shuffledAnchors[i];
            if (anchor != null)
            {
                anchor.SetActive(true);
                currentActiveAnchors++;

                ShieldAnchor anchorScript = anchor.GetComponent<ShieldAnchor>();
                if (anchorScript == null) anchorScript = anchor.AddComponent<ShieldAnchor>();

                anchorScript.Setup(this);

                StartCoroutine(SpawnSimpleBeam(anchor.transform));
            }
        }

        if (shieldObject != null)
        {
            shieldObject.SetActive(true);

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

        while (currentActiveAnchors > 0)
        {
            yield return null;
        }

        DeactivateShield();
    }

    IEnumerator SpawnSimpleBeam(Transform anchorTransform)
    {
        if (beam3DPrefab == null || bossCenter == null) yield break;

        GameObject beam = Instantiate(beam3DPrefab, anchorTransform.position, Quaternion.identity);
        activeBeams.Add(beam);

        beam.transform.LookAt(bossCenter);

        while (isShieldActive && beam != null && anchorTransform != null && anchorTransform.gameObject.activeInHierarchy)
        {
            beam.transform.LookAt(bossCenter);
            beam.transform.position = anchorTransform.position;
            yield return null;
        }

        if (beam != null)
        {
            activeBeams.Remove(beam);
            Destroy(beam);
        }
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

        foreach (GameObject beam in activeBeams)
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