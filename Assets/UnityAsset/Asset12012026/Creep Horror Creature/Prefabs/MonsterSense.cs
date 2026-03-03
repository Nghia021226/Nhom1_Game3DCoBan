using UnityEngine;
using Unity.Behavior;

public class MonsterSenses : MonoBehaviour
{
    [Header("Cài đặt")]
    public float detectionRadius = 15f;
    public LayerMask foodLayer; 
    public string playerTag = "Player"; 

    private BehaviorGraphAgent behaviorAgent;

    void Start()
    {
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
    }

    void Update()
    {
        if (behaviorAgent == null) return;

        DetectFood();
        DetectPlayerByTag(); 
    }

    
    void DetectPlayerByTag()
    {
        GameObject foundPlayer = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(playerTag))
            {
                foundPlayer = hit.gameObject;
                break; 
            }
        }

        
        if (foundPlayer != null)
        {
            behaviorAgent.SetVariableValue("TargetPlayer", foundPlayer);
        }
        else
        {
            
            behaviorAgent.SetVariableValue("TargetPlayer", (GameObject)null);
        }
    }

    void DetectFood()
    {
        Collider[] foods = Physics.OverlapSphere(transform.position, detectionRadius, foodLayer);
        if (foods.Length > 0)
        {
            behaviorAgent.SetVariableValue("TargetFood", foods[0].gameObject);
        }
        else
        {
            behaviorAgent.SetVariableValue("TargetFood", (GameObject)null);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}