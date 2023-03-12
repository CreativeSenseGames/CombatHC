using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI timer;
    public TMPro.TextMeshProUGUI nbCoin;
    public GameObject panelWin;
    public GameObject panelLose;


    /// <summary>
    /// In case of losing the game, show the winning panel.
    /// </summary>
    public void ShowWin()
    {
        panelWin.SetActive(true);
    }

    /// <summary>
    /// In case of losing the game, show the lose panel.
    /// </summary>
    public void ShowLose()
    {
        panelLose.SetActive(true);
    }

    /// <summary>
    /// Update the text object with the right timer.
    /// </summary>
    public void UpdateTime(float time)
    {
        float nbMinute = time / 60;
        float nbSeconds = time% 60;

        string timerString = "";
        if (nbMinute < 10) timerString += "0";
        timerString += (int)(nbMinute) + ":";
        if (nbSeconds < 10) timerString += "0";
        timerString += (int)(nbSeconds);
        timer.text = timerString;
    }

    /// <summary>
    /// Update the text object with the right amount of coin.
    /// </summary>
    public void UpdateCoin(int coin)
    {
        nbCoin.text = coin + "";
    }

    /// <summary>
    /// In case of pressing the restart button, load the scene with the same scene index.
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// In case of pressing the continue button, load the scene with the next scene index.
    /// </summary>
    public void ContinueLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
