using UnityEngine;

public class bulletProjectTile : MonoBehaviour
{
   private Rigidbody bulletRigidbody;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private float speed;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //float speed = 20f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable victim = other.GetComponent<IDamageable>();

        if (victim != null)
        {
            victim.TakeDamage(25f); 
            if (vfxHitGreen != null) Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            if (vfxHitRed != null) Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
