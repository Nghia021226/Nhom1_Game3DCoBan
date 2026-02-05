using UnityEngine;

public class BulletMover : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    //    void OnTriggerEnter(Collider other)
    //    {
    //        if (other.CompareTag("Player"))
    //        {
    //            // Ví dụ Player có script PlayerHealth
    //            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    //            Destroy(gameObject);
    //        }

    //        if (other.CompareTag("Wall"))
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
