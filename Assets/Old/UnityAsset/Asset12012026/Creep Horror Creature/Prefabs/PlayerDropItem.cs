using UnityEngine;

public class PlayerDropItem : MonoBehaviour
{
    [Header("Cài đặt Vị Trí")]
    public Transform dropPoint;
    public float dropOffset = 0.5f;

    [Header("Cài đặt Góc Xoay (Quan trọng)")]
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

       
        Vector3 spawnPos = transform.position + (transform.forward * 1.0f) + (Vector3.up * dropOffset);

       
        Instantiate(GameManager.instance.meatPrefab, spawnPos, Quaternion.Euler(spawnRotation));

      
        GameManager.instance.hasMeat = false;
        Debug.Log("Đã thả xương xuống đất!");
    }
}