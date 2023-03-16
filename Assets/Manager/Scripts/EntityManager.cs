using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing the behavior of all the enemies and the coins left by the monsters.
/// </summary>
public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    public EntityPathfinder entityPathfinder;

    public Transform enemyContainer;
    List<Enemy> listAllEnemies;

    public Transform coinContainer;
    List<GameObject> listAllCoins;

    public Transform fakePvpContainer;
    List<SquadManager> listAllSquads;

    public SquadManager squadManagerPlayer;

    void Awake()
    {
        instance = this;
        listAllEnemies = new List<Enemy>();
        listAllCoins = new List<GameObject>();
        listAllSquads = new List<SquadManager>();
        listAllSquads.Add(squadManagerPlayer);
    }

    void Update()
    {
        //If the game is over, don't do anything
        if (GameManager.instance.IsGameOver()) return;

        //If an ennemy is in rnage of the squad, it will store in this list, to be killed at the end of the update loop.
        List<Enemy> enemiesToKill = new List<Enemy>();

        //We makes each enemy moves ind irection of the squad.
        foreach (Enemy enemy in listAllEnemies)
        {

            enemy.timeAttack -= Time.deltaTime;

            Vector3 closestSquadPosition = squadManagerPlayer.transform.position;
            float distanceToClosestSquad = Vector3.SqrMagnitude(enemy.transform.position - squadManagerPlayer.transform.position) - squadManagerPlayer.GetRadiusSquad()* squadManagerPlayer.GetRadiusSquad();
            SquadManager squadToAttack = squadManagerPlayer;
            foreach (SquadManager squad in listAllSquads)
            {
                if (!squad.isPlayer && squad.nbCharacters!=0)
                {
                    float distanceToSquad = Vector3.SqrMagnitude(enemy.transform.position - squad.transform.position) - squad.GetRadiusSquad() * squad.GetRadiusSquad();
                    if(distanceToSquad< distanceToClosestSquad)
                    {
                        closestSquadPosition = squad.transform.position;
                        squadToAttack = squad;
                    }
                }
            }

            //We compare the distance between the enemy and the squad with the radius and the attack range, to see if the enemy is in range of attack.
            float distanceToAttackSquad = squadToAttack.GetRadiusSquad() + enemy.settingsEnemy.enemyAtkRange;
            if (new Vector2(enemy.transform.position.x - squadToAttack.transform.position.x, enemy.transform.position.z - squadToAttack.transform.position.z).sqrMagnitude < distanceToAttackSquad * distanceToAttackSquad)
            {
                if(enemy.timeAttack<0)
                {
                    //If the enemy is in range, the squad is attacked
                    squadToAttack.DoDamageSquad(enemy.transform.position, enemy.settingsEnemy.enemyAtkDamage);

                    enemy.timeAttack = 1 / enemy.settingsEnemy.attackSpeed;
                }
                
            }
            else
            {

                //We find the position to go for this enemy using the Enemy pathfinder script.
                Vector3 positionToGo = entityPathfinder.ClampPosition(entityPathfinder.GetPositionToGoPathfinding(enemy.transform.position, closestSquadPosition, enemy.settingsEnemy.enemyRadius, enemy.GetRandomValue()));

                //We find the angle direction from the current position.
                float angleToGo = 90f - Mathf.Rad2Deg * Mathf.Atan2(positionToGo.z - enemy.transform.position.z, positionToGo.x - enemy.transform.position.x);
                //We rotate the ennemi in direction of the right angle.
                enemy.transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(enemy.transform.eulerAngles.y, angleToGo, enemy.settingsEnemy.enemyTurnSpeed * Time.deltaTime), 0);

                //We check the difference of angle between the new angle and right one
                float diffAngle = enemy.transform.eulerAngles.y - angleToGo;
                if (diffAngle > 180f) diffAngle -= 360f;
                else if (diffAngle < -180f) diffAngle += 360f;

                //We moves the ennemy only if its current angle is close to the right one.
                if (Mathf.Abs(diffAngle) < 90f)
                {
                    //We move the ennemy in the direction he is looking at.
                    Vector3 nextPosition = enemy.transform.position + enemy.settingsEnemy.enemyMoveSpeed * Time.deltaTime * enemy.transform.forward;
                    enemy.transform.position = entityPathfinder.ClampPosition(nextPosition);
                }
            }
        }

        //We kill the enemy that should have been killed.
        foreach(Enemy enemyToKill in enemiesToKill)
        {
            enemyToKill.TakeDamage(100000);
        }

        for(int i= listAllSquads.Count-1; i>=0; i--)
        {
            SquadManager squad = listAllSquads[i];
            if (!squad.isPlayer)
            {
                if(squad.nbCharacters==0)
                {
                    listAllSquads.RemoveAt(i);
                    Destroy(squad.gameObject, 2f);
                }
                else
                {
                    squad.SetPrevPosition(squad.transform.position);
                    float distanceToPlayer = Vector3.SqrMagnitude(squad.transform.position - squadManagerPlayer.transform.position);
                    if (distanceToPlayer < (squad.GetRadiusSquad() + squadManagerPlayer.GetRadiusSquad() + squad.GetRangeWeapon() * 0.5f) * (squad.GetRadiusSquad() + squadManagerPlayer.GetRadiusSquad() + squad.GetRangeWeapon() * 0.5f))
                    {

                    }
                    else
                    {
                        //We find the position to go for this enemy using the Enemy pathfinder script.
                        Vector3 positionToGo = entityPathfinder.ClampPosition(entityPathfinder.GetPositionToGoPathfinding(squad.transform.position, squadManagerPlayer.transform.position, squad.GetRadiusSquad(), 0f));

                        squad.transform.position = Vector3.MoveTowards(squad.transform.position, positionToGo, squad.squadSettings.squadMoveSpeed * Time.deltaTime);
                        squad.CheckPositionSquad(new Vector2(positionToGo.x - squad.transform.position.x, positionToGo.z - squad.transform.position.z));
                    }
                }
                
            }



            //We check for each coin if it is in range of the squad
            float distanceGetCoin = squad.GetRadiusSquad() * squad.GetRadiusSquad();
            for (int c = listAllCoins.Count - 1; c >= 0; c--)
            {
                GameObject coin = listAllCoins[c];
                if (new Vector2(coin.transform.position.x - squad.transform.position.x, coin.transform.position.z - squad.transform.position.z).sqrMagnitude < distanceGetCoin)
                {
                    //If it is in range, we destory the coin gameobject and we increase the amount of coin in the suqad manager.
                    squad.AddCoin(1);
                    listAllCoins.RemoveAt(c);
                    Destroy(coin.gameObject);
                }
            }

        }
        


    }

    /// <summary>
    /// Return the closest enemy from position if any is in range.
    /// </summary>
    public Entity FindClosestEnemyInRange(Vector3 position, float range, SquadManager squad)
    {
        Entity closestEnnemy = null;
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
        foreach(SquadManager squadOther in listAllSquads)
        {
            if(squadOther!=squad)
            {
                Entity closerEntitySquad = squadOther.GetCharacterCloserFromAttackerPosition(position);
                float distanceSqr = new Vector2(closerEntitySquad.transform.position.x - position.x, closerEntitySquad.transform.position.z - position.z).sqrMagnitude;
                //We comapre the squared values to avoid to use square root.
                if (distanceSqr < minDistance)
                {
                    minDistance = distanceSqr;
                    closestEnnemy = closerEntitySquad;
                }
            }
        }

        //return the closest ennemy if one is in range.
        return closestEnnemy;
    }

    /// <summary>
    /// Return the closest enemy in range from position and all the enemies within rangeAOE of this enemy
    /// </summary>
    public List<Entity> FindClosestEnemiesInRangeWithAOE(Vector3 position, float range, float rangeAOE, SquadManager squad)
    {
        //We get the closest enemy in range of position.
        Entity closestEnnemy = FindClosestEnemyInRange(position, range, squad);


        
        //if no enemy is found, that we return an empty list.
        if(closestEnnemy==null) return new List<Entity>();

        //If an enemy is found, we will add i the list this enemy and all the enemy in rangeAOE of this enemy.
        List<Entity> closestEnnemiesWithAOE = new List<Entity>();
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
        foreach (SquadManager squadOther in listAllSquads)
        {
            if (squadOther != squad)
            {
                float distanceSqrSquad = new Vector2(squadOther.transform.position.x - closestEnnemy.transform.position.x, squadOther.transform.position.z - closestEnnemy.transform.position.z).sqrMagnitude;
                if(distanceSqrSquad < squadOther.GetRadiusSquad() * squadOther.GetRadiusSquad())
                {
                    for(int i=0; i<squadOther.charactersOfEachType.Count;i++)
                    {
                        for (int j = 0; j < squadOther.charactersOfEachType[i].Count;j++)
                        {
                            float distanceSqr = new Vector2(squadOther.charactersOfEachType[i][j].transform.position.x - closestEnnemy.transform.position.x, squadOther.charactersOfEachType[i][j].transform.position.z - closestEnnemy.transform.position.z).sqrMagnitude;
                            //We comapre the squared values to avoid to use square root.
                            if (distanceSqr < rangeAOE * rangeAOE)
                            {
                                closestEnnemiesWithAOE.Add(squadOther.charactersOfEachType[i][j]);
                            }
                        }
                    }
                }

            }
        }



        return closestEnnemiesWithAOE;
    }

    public void AddEnemy(Enemy enemy)
    {
        listAllEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        listAllEnemies.Remove(enemy);
    }
    
    public void AddCoin(GameObject coin)
    {
        listAllCoins.Add(coin);
    }

    public List<SquadManager> GetListAllSquads()
    {
        return listAllSquads;
    }

    /// <summary>
    /// Create the fake pvp squad at the right position.
    /// </summary>
    public void SpawnSquad(SquadSettingsScriptableObject settingsSquad, WeaponScriptableObject weaponStartSquad, float minDistance)
    {
        GameObject newSquad = new GameObject();
        newSquad.transform.parent = fakePvpContainer;
        GameObject parentContainer = new GameObject();
        parentContainer.transform.parent = newSquad.transform;

        SquadManager squad = newSquad.AddComponent<SquadManager>();
        squad.isPlayer = false;
        squad.squadSettings = settingsSquad;
        squad.characterParent = parentContainer.transform;
        squad.listClassCharacters = new List<ScriptableObjectClass>();
        foreach(ScriptableObjectClass soClass in squadManagerPlayer.listClassCharacters)
        {
            squad.listClassCharacters.Add(soClass);
        }
        squad.nbCharacters = 0; Vector3 getPositionStartSquad = entityPathfinder.GetPositonStartSquad(squadManagerPlayer.transform.position, squad.GetRadiusSquad(), minDistance);

        squad.transform.position = getPositionStartSquad;
        squad.CheckPositionSquad(Vector2.zero);

        squad.InitSquad();
        if (weaponStartSquad != null) squad.ChangeWeapon(weaponStartSquad);

        listAllSquads.Add(squad);


       
        
        
    }

}
