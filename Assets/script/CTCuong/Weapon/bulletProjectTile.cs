using UnityEngine;

public class bulletProjectTile : MonoBehaviour
{
   private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 10f;
        bulletRigidbody.linearVelocity = transform.forward * speed;
    }
}
