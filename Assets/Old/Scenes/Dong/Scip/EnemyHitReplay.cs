using UnityEngine;

public class EnemyHitReplay : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnHit()
    {
        anim.SetBool("Hit", true);
        Invoke(nameof(ResetHit), 0.3f);
    }

    void ResetHit()
    {
        anim.SetBool("Hit", false);
    }
}
