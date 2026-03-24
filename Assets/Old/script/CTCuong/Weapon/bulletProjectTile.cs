using UnityEngine;

public class bulletProjectTile : MonoBehaviour
{
   private Rigidbody bulletRigidbody;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private float speed;
    [SerializeField] private float damage = 25f;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable victim = other.GetComponent<IDamageable>();

        if (victim != null)
        {
            victim.TakeDamage(damage); 
            if (vfxHitGreen != null) Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            if (vfxHitRed != null) Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
