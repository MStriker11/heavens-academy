using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime; //access Player
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameTxt, kdaTxt;
    string numKills = "00", numDeaths = "00";

    Player player;
    public void Initialize(Player player)
    {
        usernameTxt.text = player.NickName;
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
        kdaTxt.text = numKills + " / " + numDeaths;
    }
}
