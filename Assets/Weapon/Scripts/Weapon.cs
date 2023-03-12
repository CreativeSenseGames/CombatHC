using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// The transform parent for the shot effect, holds the position and orientation of the effects.
    /// </summary>
    public Transform shootEffectParent;

    /// <summary>
    /// This prefab used to show that this wepaon is shooting.
    /// </summary>
    public GameObject effectShoot;

    /// <summary>
    /// Method called to play the visual effect of this weapon in case of shoot.
    /// </summary>
    public void Shoot()
    {
        //Instantiate the effect prefab.
        GameObject newEffectShoot = GameObject.Instantiate(effectShoot, shootEffectParent);
        
        //Set the position and rotatin at 0, same as the transform parent.
        newEffectShoot.transform.localPosition = Vector3.zero;
        newEffectShoot.transform.localEulerAngles = Vector3.zero;

        //Force the object to destroy after 0.5s.
        Destroy(newEffectShoot, 0.5f);
    }


}
