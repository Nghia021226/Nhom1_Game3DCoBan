using UnityEngine;

public class BulletMover : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 10;

    private float fixedY; // Lưu độ cao cố định

    void Start()
    {
        // Lưu lại độ cao lúc vừa được tạo ra từ Fire Point
        fixedY = transform.position.y;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 1. Di chuyển về phía trước
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 2. Khóa trục Y: Giữ nguyên vị trí Y cũ, chỉ thay đổi X và Z
        Vector3 currentPos = transform.position;
        currentPos.y = fixedY;
        transform.position = currentPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            Destroy(gameObject);
        }
    }
}