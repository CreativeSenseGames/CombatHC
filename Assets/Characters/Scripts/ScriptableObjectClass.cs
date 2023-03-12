using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectClass", order = 1)]
public class ScriptableObjectClass : ScriptableObject
{
    public CharacterClass characterClass;

    public GameObject prefabCharacter;

    public float characterHealthStart;
    public float characterHealthPerLevel;
    public bool hasAbility;
    [ShowIf("hasAbility")]
    public float chAbilityCoolDown;
    [ShowIf("hasAbility")]
    [Tooltip("Range of the ability : for the medic, it is not used, for the rocket soldier, it is the range to target an ennemy.")]
    public float chAbilityRange;
    [Tooltip("Radius of the ability : for the medic, it is the radius in which other character will gets healed, for the rocket soldier, it is the radius in which enemy will get damage.")]
    public float chAbilityRadius;
    [ShowIf("hasAbility")]
    [Tooltip("Strength value of ability : for the medic, it is the heal value, for the rocket soldier, it is the damage dealt.")]
    public float chAbilityStrength;
    [ShowIf("hasAbility")]
    public GameObject prefabAbility;

    public bool canHaveWeapon;
    [ShowIf("canHaveWeapon")]
    public WeaponScriptableObject startingWeapon;
}