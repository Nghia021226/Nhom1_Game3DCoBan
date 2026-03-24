using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform3D : MonoBehaviour
{
    [Header("Cài đặt Di Chuyển")]
    [SerializeField] private Transform[] points; 
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waitTime = 0.5f; 

    private int currentTargetIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        if (points.Length > 0)
        {
            transform.position = points[0].position;
        }
    }
    void FixedUpdate()
    {
        if (points.Length == 0 || isWaiting) return;
        Transform targetPoint = points[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAndNextPoint());
        }
    }
    IEnumerator WaitAndNextPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentTargetIndex++;
        if (currentTargetIndex >= points.Length)
        {
            currentTargetIndex = 0;
        }
        isWaiting = false;
    }
}