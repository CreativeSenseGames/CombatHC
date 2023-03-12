using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the Rocket soldier. 
/// </summary>
public class RocketSoldier : Character
{
    /// <summary>
    /// This method override the one of Character and makes the rocket soldier to shoot a rocket
    /// that will deal damage to every enemy in a radius of the explosion.
    /// </summary>
    public override bool DoAbility()
    {
        //Get the closest enemy and all enemy aroiunt it in the radius of the ability.
        List<Enemy> closestEnnemyInRangeWithAOE = EnemyManager.instance.FindClosestEnemiesyInRangeWithAOE(this.transform.position, settingsCharacter.chAbilityRange, settingsCharacter.chAbilityRadius);

        if (closestEnnemyInRangeWithAOE.Count>0)
        {

            //Create a sphere effect on the enemy target to show the explosion.
            GameObject ability = GameObject.Instantiate(settingsCharacter.prefabAbility);
            ability.transform.position = closestEnnemyInRangeWithAOE[0].transform.position;
            ability.AddComponent<SimpleGettingBigger>().sizeFinal = settingsCharacter.chAbilityRadius;
            ability.GetComponent<SimpleGettingBigger>().speedToGetSize = 5f;
            Destroy(ability, 1f);

            //Deal damage to all the enemy in range of the explosion.
            foreach(Enemy enemy in closestEnnemyInRangeWithAOE)
            {
                enemy.TakeDamage(settingsCharacter.chAbilityStrength);
            }

            return true;
        }
        return false;
    }
}
