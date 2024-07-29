using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomItem : MonoBehaviour
{
    public Text roomName;
    CreateAndJoinRooms manager;
    
    private void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    public void setRoomName(String rN)
    {
        roomName.text = rN;
    }

    public void onClickItem()
    {
        manager.joinRoom(roomName.text);
    }
}
