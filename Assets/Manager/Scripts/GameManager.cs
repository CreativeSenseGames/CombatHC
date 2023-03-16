using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    SquadManager playerSquad;
    public float timeGame;

    bool isOver = false;
    int nbCoin = 0;
    public UIManager uiManager;

    void Awake()
    {
        instance = this;
        playerSquad = this.transform.GetComponent<SquadManager>();
        timeGame = 0f;
    }

    void Update()
    {
        //Update the timer.
        timeGame += Time.deltaTime;
        uiManager.UpdateTime(timeGame);


        //Check the game over conditions.
        if (timeGame > playerSquad.squadSettings.timeToSurviveToWin)
        {
            uiManager.ShowWin();
            isOver = true;
        }
        else if (playerSquad.nbCharacters <= 0)
        {
            isOver = true;
            uiManager.ShowLose();
        }
    }

    /// <summary>
    /// Add a coin to the coin number
    /// </summary>
    public void AddCoin(int nbCoinToAdd)
    {
        nbCoin += nbCoinToAdd;
        uiManager.UpdateCoin(nbCoin);
    }

    /// <summary>
    /// Return if the game is over.
    /// </summary>
    public bool IsGameOver()
    {
        return isOver;
    }
}
