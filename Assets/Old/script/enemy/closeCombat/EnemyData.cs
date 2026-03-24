using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public float damage = 20f;
}