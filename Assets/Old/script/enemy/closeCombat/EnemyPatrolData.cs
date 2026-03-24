using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolData : MonoBehaviour
{
    [Header("Kéo thả các điểm Waypoint vào đây")]
    public List<Transform> patrolPoints; 

    private int _lastIndex = -1; 

    public Vector3 GetRandomWaypoint()
    {
        if (patrolPoints.Count == 0) return transform.position;

        
        if (patrolPoints.Count == 1) return patrolPoints[0].position;

        int newIndex = Random.Range(0, patrolPoints.Count);

        
        while (newIndex == _lastIndex)
        {
            newIndex = Random.Range(0, patrolPoints.Count);
        }

        _lastIndex = newIndex;

        return patrolPoints[newIndex].position;
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            if (patrolPoints[i] != null)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.5f);
                
                Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Count];
                if (nextPoint != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
                }
            }
        }
    }
 }