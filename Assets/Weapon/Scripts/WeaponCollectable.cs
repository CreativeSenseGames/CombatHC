using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollectable : MonoBehaviour
{
    public WeaponScriptableObject weaponToCollect;
    public Transform weaponPrefabContainer;

    float speedRotation = 180f;
    float timeRotation = 0f;
    public void OnDrawGizmos()
    {
        for(int i=0; i< weaponPrefabContainer.childCount; i++)
        {
            DestroyImmediate(weaponPrefabContainer.GetChild(i).gameObject);
        }
        if(weaponToCollect!=null && weaponToCollect.prefab!=null)
        {
            GameObject weaponGo = GameObject.Instantiate(weaponToCollect.prefab, weaponPrefabContainer);
            weaponGo.transform.localPosition = 0.5f * Vector3.up;
            weaponGo.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void Update()
    {
        timeRotation += speedRotation * Time.deltaTime;
        weaponPrefabContainer.transform.eulerAngles = timeRotation * Vector3.up;


        float distanceMinToGetWeapon = SquadManager.instance.GetRadiusSquad();

        if(new Vector2(this.transform.position.x - SquadManager.instance.transform.position.x, this.transform.position.z - SquadManager.instance.transform.position.z).sqrMagnitude < 0.5f * distanceMinToGetWeapon* distanceMinToGetWeapon)
        {
            SquadManager.instance.ChangeWeapon(weaponToCollect);
            Destroy(this.gameObject);
        }

    }
}
