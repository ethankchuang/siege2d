using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomItem : MonoBehaviour
{
    public Text roomName;
    CreateAndJoinRooms manager;
    public ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
    
    private void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
        properties["inGame"] = false;
    }

    public void setRoomName(String rN)
    {
        roomName.text = rN;
    }

    public void onClickItem()
    {
        if (!(bool)properties["inGame"]) {
            manager.joinRoom(roomName.text);
        } else {
            Debug.Log("already in game");
        }
    }
}
