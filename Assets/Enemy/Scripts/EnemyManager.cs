using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing the behavior of all the enemies and the coins left by the monsters.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public EnemyPathfinder enemyPathfinder;

    public Transform enemyContainer;
    public List<Enemy> listAllEnemies;

    public Transform coinContainer;
    public List<GameObject> listAllCoins;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        //If the game is over, don't do anything
        if (SquadManager.instance.IsGameOver()) return;

        //If an ennemy is in rnage of the squad, it will store in this list, to be killed at the end of the update loop.
        List<Enemy> enemiesToKill = new List<Enemy>();

        //We makes each enemy moves ind irection of the squad.
        foreach (Enemy enemy in listAllEnemies)
        {
            //We find the position to go for this enemy using the Enemy pathfinder script.
            Vector3 positionToGo = enemyPathfinder.ClampPosition(enemyPathfinder.GetPositionToGoPathfinding(enemy.transform.position, SquadManager.instance.transform.position, enemy.GetRandomValue()));
            
            //We find the angle direction from the current position.
            float angleToGo = 90f - Mathf.Rad2Deg * Mathf.Atan2(positionToGo.z - enemy.transform.position.z, positionToGo.x - enemy.transform.position.x);
            //We rotate the ennemi in direction of the right angle.
            enemy.transform.eulerAngles =new Vector3(0,Mathf.MoveTowardsAngle(enemy.transform.eulerAngles.y, angleToGo, enemy.settingsEnemy.enemyTurnSpeed*Time.deltaTime), 0);

            //We check the difference of angle between the new angle and right one
            float diffAngle = enemy.transform.eulerAngles.y - angleToGo;
            if (diffAngle > 180f) diffAngle -= 360f;
            else if (diffAngle < -180f) diffAngle += 360f;

            //We moves the ennemy only if its current angle is close to the right one.
            if(Mathf.Abs(diffAngle)<45f)
            {
                //We move the ennemy in the direction he is looking at.
                Vector3 nextPosition = enemy.transform.position + enemy.settingsEnemy.enemyMoveSpeed * Time.deltaTime * enemy.transform.forward;
                enemy.transform.position = enemyPathfinder.ClampPosition(nextPosition);
            }

            //We compare the distance between the enemy and the squad with the radius and the attack range, to see if the enemy is in range of attack.
            float distanceToAttackSquad = SquadManager.instance.GetRadiusSquad() + enemy.settingsEnemy.enemyAtkRange;
            if (new Vector2(enemy.transform.position.x - SquadManager.instance.transform.position.x, enemy.transform.position.z - SquadManager.instance.transform.position.z).sqrMagnitude< distanceToAttackSquad* distanceToAttackSquad)
            {
                //If the enemy is in range, the squad is attacked
                enemiesToKill.Add(enemy);
                //and the enemy is killed.
                SquadManager.instance.DoDamageSquad(enemy.transform.position, enemy.settingsEnemy.enemyAtkDamage);
            }

        }

        //We kill the enemy that should have been killed.
        foreach(Enemy enemyToKill in enemiesToKill)
        {
            enemyToKill.TakeDamage(100000);
        }

        //We check for each coin if it is in range of the squad
        float distanceGetCoin = SquadManager.instance.GetRadiusSquad()* SquadManager.instance.GetRadiusSquad();
        for (int i=listAllCoins.Count-1; i>=0; i--)
        {
            GameObject coin = listAllCoins[i];
            if (new Vector2(coin.transform.position.x - SquadManager.instance.transform.position.x, coin.transform.position.z - SquadManager.instance.transform.position.z).sqrMagnitude < distanceGetCoin)
            {
                //If it is in range, we destory the coin gameobject and we increase the amount of coin in the suqad manager.
                SquadManager.instance.AddCoin(1);
                listAllCoins.RemoveAt(i);
                Destroy(coin.gameObject);
            }
        }


    }

    /// <summary>
    /// Return the closest enemy from position if any is in range.
    /// </summary>
    public Enemy FindClosestEnemyInRange(Vector3 position, float range)
    {
        Enemy closestEnnemy = null;
        //We initialise the distance at range, so we only return an enemy if it is in the range.
        float minDistance = range * range;
        foreach (Enemy enemy in listAllEnemies)
        {
            float distanceSqr = new Vector2(enemy.transform.position.x - position.x, enemy.transform.position.z - position.z).sqrMagnitude;
            //We comapre the squared values to avoid to use square root.
            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                closestEnnemy = enemy;
            }
        }

        //return the closest ennemy if one is in range.
        return closestEnnemy;
    }

    /// <summary>
    /// Return the closest enemy in range from position and all the enemies within rangeAOE of this enemy
    /// </summary>
    public List<Enemy> FindClosestEnemiesyInRangeWithAOE(Vector3 position, float range, float rangeAOE)
    {
        //We get the closest enemy in range of position.
        Enemy closestEnnemy = FindClosestEnemyInRange(position, range);


        
        //if no enemy is found, that we return an empty list.
        if(closestEnnemy==null) return new List<Enemy>();

        //If an enemy is found, we will add i the list this enemy and all the enemy in rangeAOE of this enemy.
        List<Enemy> closestEnnemiesWithAOE = new List<Enemy>();
        closestEnnemiesWithAOE.Add(closestEnnemy);

        //We loop on every enemy
        foreach (Enemy enemy in listAllEnemies)
        {
            if (enemy == closestEnnemy) continue;
            float distanceSqr = new Vector2(enemy.transform.position.x - closestEnnemy.transform.position.x, enemy.transform.position.z - closestEnnemy.transform.position.z).sqrMagnitude;
            //We comapre the squared values to avoid to use square root.
            if (distanceSqr < rangeAOE* rangeAOE)
            {
                closestEnnemiesWithAOE.Add(enemy);
            }
        }


        

        return closestEnnemiesWithAOE;
    }

   
}
