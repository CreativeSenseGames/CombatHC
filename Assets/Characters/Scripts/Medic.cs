using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : Character
{
    /// <summary>
    /// This method override the one of Character and makes the medic soldier to heal every character around him.
    /// </summary>
    public override bool DoAbility()
    {
        //Get All the characters in the ability radius
        List<Character> charactersInRange = SquadManager.instance.GetCharacterAround(this.transform.position, settingsCharacter.chAbilityRadius);

        //Create a sphere effect on the medic to show the healing ability.
        GameObject ability = GameObject.Instantiate(settingsCharacter.prefabAbility, this.transform);
        ability.transform.position = this.transform.position;
        ability.AddComponent<SimpleGettingBigger>().sizeFinal = settingsCharacter.chAbilityRadius;
        ability.GetComponent<SimpleGettingBigger>().speedToGetSize = 5f;
        Destroy(ability, 1f);

        //Heal eveyr character in the range of the ability.
        foreach (Character character in charactersInRange)
        {
            character.Heal(settingsCharacter.chAbilityStrength);
        }

        return true;
    }
}
