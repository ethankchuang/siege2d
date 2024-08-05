using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ListedPlayerScript : MonoBehaviourPunCallbacks
{
    public Text playerName;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    Player player;
    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void setIsDef(bool value)
    {
        playerProperties["isDef"] = value;
        //Debug.Log(playerProperties["isDef"] + " is def");
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer) {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player _player) 
    {
        if (_player.CustomProperties.ContainsKey("isDef"))
        {
            playerProperties["isDef"] = (bool)_player.CustomProperties["isDef"];
        }
    }
}
