using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to put on the character prefab to specify where to instanciate the weapon on the character.
/// </summary>
public class WeaponHolding : MonoBehaviour
{
    public Transform parentWeapon;
    public Vector3 offsetPosition;
    public Vector3 offsetEulerAngles;
}
