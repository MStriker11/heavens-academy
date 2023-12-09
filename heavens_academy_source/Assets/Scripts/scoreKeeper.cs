using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime; //access Player
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class scoreKeeper : MonoBehaviourPunCallbacks
{
    #region score
    public TMP_Text allyScoreTxt, enemyScoreTxt;
    string numKills = "00", numDeaths = "00";

    Player player;
    public void Initialize(Player player)
    {
        this.player = player;
        updateStats();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // if scoreboard item is related to player that had properties update
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
            {
                updateStats();
            }
        }
    }

    void updateStats()
    {
        // trygetValue() b/c kills might not be a custom property as it has not been incremented yet
        if (player.CustomProperties.TryGetValue("kills", out object kills))
        {
            numKills = kills.ToString();
        }
        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {
            numDeaths = deaths.ToString();
        }
        allyScoreTxt.text = numKills;
        enemyScoreTxt.text = numDeaths;
    }
    #endregion score

    #region timer
    public TMP_Text timerTxt;
    TMP_Text victoryTxt, defeatTxt;
    float timer = 300; //# seconds

    public Image victoryScreen, defeatScreen;
    bool gameEnd = false;

    private void Awake()
    {
        victoryTxt = victoryScreen.GetComponentInChildren<TMP_Text>();
        defeatTxt = defeatScreen.GetComponentInChildren<TMP_Text>();
        victoryScreen.enabled = false;
        victoryTxt.enabled = false;
        defeatScreen.enabled = false;
        defeatTxt.enabled = false;
        gameEnd = false;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
        if (gameEnd)
        {
            showEndScreen();
        }
        displayTime(timer);
    }

    void displayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
            gameEnd = true;
        }

        float min = Mathf.FloorToInt(timeToDisplay / 60);
        float sec = Mathf.FloorToInt(timeToDisplay % 60);

        timerTxt.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    void showEndScreen()
    {
        var numKillsInt = int.Parse(numKills);
        var numDeathsInt = int.Parse(numDeaths);
        Debug.Log(numKillsInt);
        if (numKillsInt > numDeathsInt)
        {
            victoryScreen.enabled = true;
            victoryTxt.enabled = true;
            defeatScreen.enabled = false;
            defeatTxt.enabled = false;
        }
        else
        {
            victoryScreen.enabled = false;
            victoryTxt.enabled = false;
            defeatScreen.enabled = true;
            defeatTxt.enabled = true;
        }
    }

    #endregion timer
}
