using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectEnemy", order = 4)]
public class ScriptableObjectEnemy : ScriptableObject
{
    public GameObject prefabEnnemy;

    public float enemyHealth;
    [Tooltip("Movement speed in units per second")]
    public float enemyMoveSpeed;
    [Tooltip("Rotation speed in degree per second")]
    public float enemyTurnSpeed;
    public float enemyAtkDamage;
    public float enemyAtkRange;
    public float enemyRadius;
    [Tooltip("The number of attack every second.")]
    public float attackSpeed;

    public int currencyLeftBehind;
    public GameObject prefabCoin;
    public float spawnCoinRadius;

}