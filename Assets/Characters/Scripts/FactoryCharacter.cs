using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FactoryCharacter
{
    /// <summary>
    /// Adds to the gameObject the right character class script from the character class.
    /// </summary>
    public static Character AddComponentCharacter(GameObject characterGameObject, CharacterClass characterClass)
    {
        if (characterClass == CharacterClass.Soldier) return characterGameObject.AddComponent<Soldier>();
        else if (characterClass == CharacterClass.Medic) return characterGameObject.AddComponent<Medic>();
        else if (characterClass == CharacterClass.Rocket_Soldier) return characterGameObject.AddComponent<RocketSoldier>();

        return null;
    }
}
