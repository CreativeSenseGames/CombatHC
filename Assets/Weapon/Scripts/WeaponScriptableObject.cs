using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponScriptableObject", order = 2)]
public class WeaponScriptableObject : ScriptableObject
{
    [Tooltip("The prefab of the weapon.")]
    public GameObject prefab;

    [Tooltip("The frame rate of the weapon: number of shoot per second.")]
    public float fireRate;
    [Tooltip("The range of this weapon.")]
    public float fireRange;
    [Tooltip("The damage caused by this weapon.")]
    public float projectileDamage;
    
}