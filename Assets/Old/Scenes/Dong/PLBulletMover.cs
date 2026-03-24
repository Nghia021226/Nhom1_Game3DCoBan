using UnityEngine;

public class PLBulletMover : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 20;
    private float fixedY;

    void Start()
    {
        // Lưu lại độ cao Y để khóa trục Y
        fixedY = transform.position.y;

        // Tự hủy sau một khoảng thời gian nếu không trúng gì
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Di chuyển về phía trước
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Khóa trục Y để đạn không bay lên trời hay xuống đất
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu chạm vào đối tượng có Tag là "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Gây sát thương cho Enemy (nếu Enemy có script EnemyHealth)
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // VIÊN ĐẠN BIẾN MẤT NGAY LẬP TỨC
            Destroy(gameObject);
        }

       
    }
}