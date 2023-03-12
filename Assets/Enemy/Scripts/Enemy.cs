using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("The scriptable settings of this enemy")]
    public ScriptableObjectEnemy settingsEnemy;
    float health;

    //This value is used for the pathfinder, to have some variation in the position of the ennemy 
    //and avoid that all enemies are on the same point.
    float randomValue;

    /// <summary>
    /// Initialize the Enemy
    /// </summary>
    public void Init()
    {
        health = settingsEnemy.enemyHealth;

        randomValue = Random.Range(-1f, 1f);

    }

    /// <summary>
    /// Decrease the health point of the damage dealt, if the health point reaches 0, calls the Death.
    /// </summary>
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health<=0)
        {
            EnemyManager.instance.listAllEnemies.Remove(this);
            this.transform.SetParent(null);
            Death();
        }
    }

    /// <summary>
    /// In case of death, instanciate the right amount of coin and play the death animation.
    /// </summary>
    public void Death()
    {
        for(int i=0; i<settingsEnemy.currencyLeftBehind; i++)
        {
            GameObject coin = GameObject.Instantiate(settingsEnemy.prefabCoin, EnemyManager.instance.coinContainer);
            EnemyManager.instance.listAllCoins.Add(coin);
            coin.transform.position = new Vector3(this.transform.position.x+Random.Range(-settingsEnemy.spawnCoinRadius, settingsEnemy.spawnCoinRadius), coin.transform.position.y, this.transform.position.z+Random.Range(-settingsEnemy.spawnCoinRadius, settingsEnemy.spawnCoinRadius));
        }
        StartCoroutine(PlayDeathAnimation());
    }

    /// <summary>
    /// Play a death animation
    /// </summary>
    private IEnumerator PlayDeathAnimation()
    {
        while (this.transform.localScale.y > 0.05f)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * (1 - 5 * Time.deltaTime), this.transform.localScale.z);
            yield return null;
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Return the random value associated to this enemy
    /// </summary>
    public float GetRandomValue()
    {
        return this.randomValue;
    }
}
