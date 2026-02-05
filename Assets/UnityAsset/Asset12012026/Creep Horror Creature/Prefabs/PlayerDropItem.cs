using UnityEngine;

public class PlayerDropItem : MonoBehaviour
{
    [Header("Cài đặt Vị Trí")]
    public Transform dropPoint;
    public float dropOffset = 0.5f;

    [Header("Cài đặt Góc Xoay (Quan trọng)")]
    // Mặc định thử để -90 hoặc 90 ở trục X. Bro chỉnh số này trong Inspector nhé!
    public Vector3 spawnRotation = new Vector3(-90, 0, 0);

    void Update()
    {
        if (GameManager.instance.hasMeat && Input.GetKeyDown(KeyCode.F))
        {
            DropMeat();
        }
    }

    void DropMeat()
    {
        if (GameManager.instance.meatPrefab == null)
        {
            Debug.LogError("Chưa kéo Meat Prefab vào GameManager kìa bro!");
            return;
        }

        // 1. Tính vị trí thả
        Vector3 spawnPos = transform.position + (transform.forward * 1.0f) + (Vector3.up * dropOffset);

        // 2. TẠO RA VỚI GÓC XOAY TÙY CHỈNH (Thay vì Quaternion.identity)
        // Quaternion.Euler biến 3 số (x,y,z) thành góc xoay
        Instantiate(GameManager.instance.meatPrefab, spawnPos, Quaternion.Euler(spawnRotation));

        // 3. Reset
        GameManager.instance.hasMeat = false;
        Debug.Log("Đã thả thịt xuống đất!");
    }
}