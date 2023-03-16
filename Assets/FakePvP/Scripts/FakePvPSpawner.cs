using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePvPSpawner : MonoBehaviour
{
    public SquadSettingsScriptableObject squadSettings;
    public WeaponScriptableObject currentWeaponSquad = null;

    [Tooltip("Time the squad should come in game in seconds.")]
    public float time = 1f;

    [Tooltip("Distance minimal to create the fake PvP Squad")]
    public float distanceMinimum = 30f;

    bool hasSpawnedSquad = false;
    void Update()
    {
        //If the game is over, don't do anything
        if (GameManager.instance.IsGameOver()) return;

        //If the fake pvp has not been spawned and that the time is up, let's spawn it.
        if(!hasSpawnedSquad && GameManager.instance.timeGame> time)
        {
            
            EntityManager.instance.SpawnSquad(squadSettings, currentWeaponSquad, distanceMinimum);
            hasSpawnedSquad = true;
        }
    }

}
