using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class of the different character class.
/// </summary>
public abstract class Character : Entity
{
    //The settings of this character class
    public ScriptableObjectClass settingsCharacter;

    public SquadManager squad;

    //The health point of the character
    float health;

    //The level of the character (not used)
    int level;

    //All the information about the ability of this character
    bool hasAbility;
    float chAbilityCoolDown;
    float abilityCoolDownWithModifier;
    float abilityTimer;

    //All the information about the weapon of this character
    bool canHaveWeapon;
    WeaponScriptableObject weapon;
    Weapon currentWeaponScript;
    float attackTimer;
    float attackTimerReset;

    /// <summary>
    /// Initialize a character using the information of the settings
    /// </summary>
    public void Init()
    {
        health = settingsCharacter.characterHealthStart;
        level = 1;

        hasAbility = settingsCharacter.hasAbility;
        chAbilityCoolDown = settingsCharacter.chAbilityCoolDown;
        abilityTimer = Random.Range(0f, 1f) * chAbilityCoolDown;

        canHaveWeapon = settingsCharacter.canHaveWeapon;
    
        //In case the character can has a weapon, set up the starting weapon.
        if(canHaveWeapon)
        {
            ChangeWeapon(settingsCharacter.startingWeapon);
            
        }

        //Do any special initialization of class
        InitSpecificityClass();
    }

    /// <summary>
    /// This method does initialization of the specificity of class.
    /// </summary>
    public virtual void InitSpecificityClass()
    {

    }

    public void Update()
    {
        //If the game is over, don't do anything
        if (GameManager.instance.IsGameOver()) return;

        //If this character has an ability
        if(hasAbility)
        {
            abilityTimer -= Time.deltaTime;

            if (abilityTimer < 0f)
            {
                //Try and do the ability if possible
                if (DoAbility())
                {
                    //reset the timer only if the ability has been done
                    abilityTimer = abilityCoolDownWithModifier;
                }
            }
        }
        
        //If this character has a weapon, and so can attack
        if(weapon!=null)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0f)
            {
                //Try and do the attack if possible
                if (DoAttack())
                {
                    //reset the timer only if the attack has been done
                    attackTimer = attackTimerReset;
                }
            }
        }
    }

    /// <summary>
    /// In case of death of the character, plays a death animation
    /// </summary>
    public void Death()
    {
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        while(this.transform.localScale.y>0.05f)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * (1 - 5*Time.deltaTime), this.transform.localScale.z);
            yield return null;
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// In case of death of the character, plays a death animation
    /// </summary>
    public virtual bool DoAbility()
    {
        return true;
    }

    /// <summary>
    /// Using the current weapon, find the closer enemy and deal damage to it.
    /// </summary>
    public bool DoAttack()
    {
        Entity closestEnnemyInRange = EntityManager.instance.FindClosestEnemyInRange(this.transform.position, weapon.fireRange, squad);

        if(closestEnnemyInRange!=null)
        {
            currentWeaponScript.Shoot();
            closestEnnemyInRange.TakeDamage(weapon.projectileDamage);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Apply a ability cooldown modifier (bonus/malus from the gate)
    /// </summary>
    public void SetCooldownModifier(float modifierCoolDown)
    {
        abilityCoolDownWithModifier = chAbilityCoolDown * modifierCoolDown;
        abilityTimer = Mathf.Min(abilityTimer, abilityCoolDownWithModifier);
    }

    /// <summary>
    /// Change the weapon of this character for the new weapon, update the 3D model as well.
    /// </summary>
    public void ChangeWeapon(WeaponScriptableObject weaponNew)
    {
        //If this character cant have weapon, do nothing.
        if(canHaveWeapon && weapon != weaponNew)
        {
            //Destroy the previous 3D model of weapon.
            if (currentWeaponScript != null)
            {
                Destroy(currentWeaponScript.gameObject);
            }

            //Update with the new weapon, and the new settings of the weapon.
            weapon = weaponNew;
            attackTimerReset = 1 / weapon.fireRate;
            attackTimer = Random.Range(0f, 1f) * attackTimerReset;

            WeaponHolding weaponHolding = this.GetComponent<WeaponHolding>();

            GameObject weaponGO = GameObject.Instantiate(weapon.prefab, weaponHolding.parentWeapon);
            weaponGO.transform.localPosition = weaponHolding.offsetPosition;
            weaponGO.transform.localEulerAngles = weaponHolding.offsetEulerAngles;

            currentWeaponScript = weaponGO.GetComponent<Weapon>();
        }
    }

    /// <summary>
    /// Return the health point of this character.
    /// </summary>
    public float GetHealthPoint()
    {
        return this.health;
    }

    /// <summary>
    /// Heal the character of the value and assuring that it goes not above maximum health.
    /// </summary>
    public void Heal(float healingValue)
    {
        this.health = Mathf.Min(this.settingsCharacter.characterHealthStart, this.health + healingValue);
    }

    /// <summary>
    /// Deal damage to the character and cause the death of it if needed.
    /// </summary>

    public override void TakeDamage(float damageValue)
    {
        this.health -= damageValue;
        if (health <= 0)
        {
            squad.RemoveCharacterFromSquad(this);
        }
    }
}
