using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that makes the behaviors of the Enemy spawner.
/// </summary>
public class EnnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnnemySettingsSpawn
    {
        public ScriptableObjectEnemy ennemyToSpawn;
        [Tooltip("Number of new ennemy every second")]
        public float spawnRate = 1f;
        [Tooltip("Distance maximum to the spawner where the zombie can spawn")]
        public float radiusSpawn = 0f;

        internal float timeBetweenSpawn = 0f;
        internal float timeToSpawn = 0f;
    }
    public List<EnnemySettingsSpawn> settingsSpawn;
    

    // Start is called before the first frame update
    void Awake()
    {
        //Foreach spawning settings, initialize the timer of spawn.
        foreach(EnnemySettingsSpawn settingSpawn in settingsSpawn)
        {
            if (settingSpawn.spawnRate == 0) settingSpawn.spawnRate = 1f;
            settingSpawn.timeBetweenSpawn = 1 / settingSpawn.spawnRate;
            settingSpawn.timeToSpawn = settingSpawn.timeBetweenSpawn;
        }
    }

    void Update()
    {
        //If the game is over, don't do anything
        if (SquadManager.instance.IsGameOver()) return;

        //Foreach spawning settings, check if an enemy should be spawn.
        foreach (EnnemySettingsSpawn settingSpawn in settingsSpawn)
        {
            settingSpawn.timeToSpawn -= Time.deltaTime;
            if(settingSpawn.timeToSpawn<=0)
            {
                settingSpawn.timeToSpawn = settingSpawn.timeBetweenSpawn;
                //Spawn the enemy of the settings.
                SpawnEnnemy(settingSpawn.ennemyToSpawn, settingSpawn.radiusSpawn);
            }
        }
    }

    /// <summary>
    /// Spawn one ennemy of the scriptable object randomly in a circle around the spawner object.
    /// </summary>
    public void SpawnEnnemy(ScriptableObjectEnemy ennemyToSpawn, float radiusMaxSpawn)
    {
        GameObject ennemyGO = GameObject.Instantiate(ennemyToSpawn.prefabEnnemy, EnemyManager.instance.enemyContainer);
        Enemy ennemy = ennemyGO.AddComponent<Enemy>();

        float randomAngle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        float randomDistance = UnityEngine.Random.Range(0, radiusMaxSpawn);

        //Randomize the position of spawn.
        ennemyGO.transform.position = new Vector3(this.transform.position.x + randomDistance * Mathf.Cos(randomAngle), ennemyGO.transform.position.y, this.transform.position.z + randomDistance * Mathf.Sin(randomAngle));
        //Randomize the orientation of the enemy
        ennemyGO.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360f), 0f);

        //Initialize the enemy.
        ennemy.settingsEnemy = ennemyToSpawn;
        ennemy.Init();

        //Add the enemy to the list of EnemyManager.
        EnemyManager.instance.listAllEnemies.Add(ennemy);
    }
}
