using UnityEngine;
using UnityEngine.AI;

public class EnemyResetHandler : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private NavMeshAgent _agent;
    private Animator _anim;

    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;

        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    public void ResetEnemy()
    {
        if (_agent != null)
        {
            _agent.Warp(_startPosition);
            _agent.ResetPath(); 
        }
        else
        {
            transform.position = _startPosition;
        }

        transform.rotation = _startRotation;

        if (_anim != null)
        {
            _anim.Rebind();
            _anim.Update(0f);
        }

        Debug.Log($"Đã Reset quái: {gameObject.name}");
    }
}