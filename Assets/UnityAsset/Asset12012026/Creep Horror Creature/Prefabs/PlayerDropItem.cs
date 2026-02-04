using UnityEngine;

public class PlayerDropItem : MonoBehaviour
{
    [Header("Cài đặt")]
    public Transform dropPoint; // Vị trí thả (Kéo cái chân hoặc tạo GameObject rỗng dưới chân)
    public float dropOffset = 0.5f; // Độ cao thả so với chân (để không bị kẹt xuống đất)

    void Update()
    {
        // Kiểm tra: Nếu đang cầm thịt VÀ nhấn F
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

        // 1. Tính vị trí thả (Dưới chân Player một chút về phía trước)
        Vector3 spawnPos = transform.position + (transform.forward * 1.0f) + (Vector3.up * dropOffset);

        // 2. Tạo ra cục thịt mới
        Instantiate(GameManager.instance.meatPrefab, spawnPos, Quaternion.identity);

        // 3. Reset trạng thái
        GameManager.instance.hasMeat = false;
        Debug.Log("Đã thả thịt xuống đất!");
    }
}