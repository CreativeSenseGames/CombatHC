using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that create a mini explosion to show effect visually.
/// </summary>
public class SimpleGettingBigger : MonoBehaviour
{
    public float sizeFinal;
    public float speedToGetSize;

    void Update()
    {
        //Lerp the size to get bigger up to the sizeFinal.
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, sizeFinal * Vector3.one, speedToGetSize * Time.deltaTime);
    }
}
