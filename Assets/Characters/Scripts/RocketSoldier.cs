using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the Rocket soldier. 
/// </summary>
public class RocketSoldier : Character
{
    public Weapon rocketLauncher;

    /// <summary>
    /// Special initialization of rocket launcher, gets the rocket launcher information.
    /// </summary>
    public override void InitSpecificityClass()
    {
        rocketLauncher = this.GetComponent<Weapon>();
    }

    /// <summary>
    /// This method override the one of Character and makes the rocket soldier to shoot a rocket
    /// that will deal damage to every enemy in a radius of the explosion.
    /// </summary>
    public override bool DoAbility()
    {
        //Get the closest enemy and all enemy aroiunt it in the radius of the ability.
        List<Entity> closestEnnemyInRangeWithAOE = EntityManager.instance.FindClosestEnemiesInRangeWithAOE(this.transform.position, settingsCharacter.chAbilityRange, settingsCharacter.chAbilityRadius, squad);

        if (closestEnnemyInRangeWithAOE.Count>0)
        {
            //Instantiate the effect prefab for the rocket launcher shoot
            GameObject newEffectShootRocketLauncher = GameObject.Instantiate(rocketLauncher.effectShoot, rocketLauncher.shootEffectParent);

            //Set the position and rotatin at 0, same as the transform parent.
            newEffectShootRocketLauncher.transform.localPosition = Vector3.zero;
            newEffectShootRocketLauncher.transform.localEulerAngles = Vector3.zero;

            //Force the object to destroy after 0.5s.
            Destroy(newEffectShootRocketLauncher, 0.5f);



            //Create a sphere effect on the enemy target to show the explosion.
            GameObject ability = GameObject.Instantiate(settingsCharacter.prefabAbility);
            ability.transform.position = closestEnnemyInRangeWithAOE[0].transform.position;
            ability.AddComponent<SimpleGettingBigger>().sizeFinal = settingsCharacter.chAbilityRadius;
            ability.GetComponent<SimpleGettingBigger>().speedToGetSize = 5f;
            Destroy(ability, 1f);

            //Deal damage to all the enemy in range of the explosion.
            foreach(Entity entity in closestEnnemyInRangeWithAOE)
            {
                entity.TakeDamage(settingsCharacter.chAbilityStrength);
            }

            return true;
        }
        return false;
    }
}
