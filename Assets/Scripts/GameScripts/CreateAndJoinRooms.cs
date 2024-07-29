using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public Text roomName;
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdate = 2f;
    float nextUpdateTime;

    public List<ListedPlayerScript> defPlayerList = new List<ListedPlayerScript>();
    public List<ListedPlayerScript> atkPlayerList = new List<ListedPlayerScript>();
    public ListedPlayerScript listedPlayerPrefab;
    public Transform attackList;
    public Transform defenceList;
    public GameObject playButton;

    //testing
    private List<RoomInfo> tempList = new List<RoomInfo>();

    public void Start()
    {
        PhotonNetwork.JoinLobby();
    }
    public void CreateRoom()
    {
        if (createInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { BroadcastPropsChangeToAll = true});
        }
    }

    public void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2){
            playButton.SetActive(true);
        } else {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlay()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        updatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("on room list update called");
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + timeBetweenUpdate;
       
            if (roomList.Count == 0)
            {
                Debug.Log("room list empty");
                tempList.Clear();
            } else{
                foreach (RoomInfo helpMe in roomList)
                {
                    Debug.Log(" ASIODUHAWHJKDKJHADLHKJSAKDHJAKHLJDAKLHJDAHKJLSD");
                    Debug.Log(helpMe.Name + " name      ");
                    Debug.Log(helpMe.RemovedFromList + " removed from list");
                    Debug.Log(helpMe.PlayerCount + " count");
                    if (helpMe.PlayerCount == 0 || helpMe.RemovedFromList)
                    {
                        tempList.Remove(helpMe);
                    }
                    else if (!tempList.Contains(helpMe))
                    {
                        tempList.Add(helpMe);
                    }
                }
            }
        }
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in tempList)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.setRoomName(room.Name);

            roomItemList.Add(newRoom);
        }
    }

    public void joinRoom(string rN)
    {
        Debug.Log("joinRoom called");
        PhotonNetwork.JoinRoom(rN);
    }

    public void onClickLeaveRoom()
    {     
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 0)
        {
            Debug.Log("current room player count less than 0");
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].Name == PhotonNetwork.CurrentRoom.Name)
                {
                    Debug.Log("tempList name and current room name are equal");
                    tempList.RemoveAt(i);
                    break;
                }
            }
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void updatePlayerList()
    {
        foreach (ListedPlayerScript item in defPlayerList)
        {
            Destroy(item.gameObject);
        }
        defPlayerList.Clear();

        foreach (ListedPlayerScript item in atkPlayerList)
        {
            Destroy(item.gameObject);
        }
        atkPlayerList.Clear();

        if (PhotonNetwork.CurrentRoom == null) {return;}

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            ListedPlayerScript newPlayer;
            Debug.Log(player.Key + " player key");
            if (player.Key % 2 == 1)
            {
                newPlayer = Instantiate(listedPlayerPrefab, attackList);
                atkPlayerList.Add(newPlayer);
                if (player.Value == PhotonNetwork.LocalPlayer) {
                    newPlayer.setIsDef(false);
                }
            }
            else{
                newPlayer = Instantiate(listedPlayerPrefab, defenceList);
                defPlayerList.Add(newPlayer);
                if (player.Value == PhotonNetwork.LocalPlayer) {
                    newPlayer.setIsDef(true);
                }
            }
            newPlayer.SetPlayerInfo(player.Value);
        } 
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayerList();
    }
}
