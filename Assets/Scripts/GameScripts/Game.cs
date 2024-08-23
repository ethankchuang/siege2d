using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;


public class Game : MonoBehaviour
{
    [SerializeField] GameObject defPrefab;
    [SerializeField] GameObject atkPrefab;
    public List<GameObject> defPlayerList;
    public List<GameObject> atkPlayerList;
    public List<GameObject> defAlive;
    public List<GameObject> atkAlive;
    public int defPoints;
    public int atkPoints;
    public int PointsToWin;
    private PhotonView view;
    [SerializeField] GameObject tileMap;
    public bool canEndRound;
    public bool switchSides;
    SpawnPlayers spawnPlayers;


    public void Start()
    {
        //defPlayerList = new List<GameObject>();
        //atkPlayerList = new List<GameObject>();
        defAlive = new List<GameObject>();
        atkAlive = new List<GameObject>();
        defPoints = 0;
        atkPoints = 0;
        view = GetComponent<PhotonView>();
        createPlayerLists();
        canEndRound = true;
        switchSides = false;
        spawnPlayers = GetComponent<SpawnPlayers>();
    }

    public void setAliveLists(GameObject deadPlayer) 
    {
        atkAlive.Clear();
        defAlive.Clear();
        foreach (GameObject player in FindObjectsOfType<GameObject>())
        {
            //Debug.Log("gameobject is player? " + (player.GetComponent<PlayerMovement>() != null));
            //Debug.Log((deadPlayer == player) + " is dead player?");
            if (player != deadPlayer && player.GetComponent<PlayerMovement>() != null && !player.GetComponent<PlayerMovement>().isDead)
            {
                if(!player.GetComponent<PlayerMovement>().isDef) {
                    //atkPlayerList.Add(player);
                    atkAlive.Add(player);
                }
                else {
                    //defPlayerList.Add(player);
                    defAlive.Add(player);
                }
            }
        }
        //Debug.Log("def count " + defAlive.Count);
        //Debug.Log("atk count " + atkAlive.Count);
        //prepActiveList();
    }

    public void createPlayerLists( ) 
    {
        defPlayerList.Clear();
        atkPlayerList.Clear();
        foreach (GameObject player in FindObjectsOfType<GameObject>())
        {
            //Debug.Log("gameobject is player? " + (player.GetComponent<PlayerMovement>() != null));
            if (player.GetComponent<PlayerMovement>() != null)
            {
                if(!player.GetComponent<PlayerMovement>().isDef) {
                    //atkPlayerList.Add(player);
                    atkPlayerList.Add(player);
                    Debug.Log("adding to atk player list " + player.GetComponent<PlayerMovement>().nickName );
                }
                else {
                    //defPlayerList.Add(player);
                    defPlayerList.Add(player);
                    Debug.Log("adding to def player list" + player.GetComponent<PlayerMovement>().nickName);
                }
            }
        }
        Debug.Log("def count " + defPlayerList.Count);
        Debug.Log("atk count " + atkPlayerList.Count);
        //prepActiveList();
    }
    
    public void endRoundHelper(bool defWin) {
        view.RPC(nameof(endRoundHelperHelper), RpcTarget.All, defWin);
    }
    [PunRPC]
    public void endRoundHelperHelper(bool defWin) {
        Debug.Log(canEndRound + " can end round?");

        if (!canEndRound) { return; }
        canEndRound = false;

        //view.RPC(nameof(endRound), RpcTarget.All, defWin);
        endRound(defWin);
    }

    //[PunRPC]
    public void endRound(bool defWin)
    {
        createPlayerLists();
        GameObject.Find("DefuserParent").transform.GetChild(0).gameObject.SetActive(false);
        Debug.Log("end round called!?");
        if (defWin) {
            defPoints ++;
            Debug.Log(defPoints + " def points " + PointsToWin + " points to win");
            if (defPoints >= PointsToWin) {
                Debug.Log("ending game!?!?");
                endGame(true);
                return;
            }
        } else {
            atkPoints ++;
            Debug.Log(atkPoints + " atk points " + PointsToWin + " points to win");
            if (atkPoints >= PointsToWin) {
                Debug.Log("ending game!?");
                endGame(false);
                return;
            }
        }

        if (atkPoints + defPoints == 2) {
            switchSides = true;
            Debug.Log("2 rounds passed");
        }
        Debug.Log("def count " + defPlayerList.Count);
        Debug.Log("atk count " + atkPlayerList.Count);
        foreach (GameObject player in defPlayerList) {
            Debug.Log("trying to call round end screen for def");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().roundEndScreen(defWin);
                if (switchSides) {
                    swappingPlayer();
                    Debug.Log("swapping sides");
                    switchSides = false;
                }
            }
        }
        foreach (GameObject player in atkPlayerList) {
            Debug.Log("trying to call round end screen for atk");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().roundEndScreen(!defWin);
                if (switchSides) {
                    swappingPlayer();
                    Debug.Log("swapping sides");
                    switchSides = false;
                }
            }
        }
    }
    public void swappingPlayer() {
        //if (view.IsMine) {
            int atkTemp = defPoints;
            int defTemp = atkPoints;
            defPoints = defTemp;
            atkPoints = atkTemp;
            foreach (GameObject player in defPlayerList) {
                player.GetComponent<PlayerMovement>().swapSides();
            }
            foreach (GameObject player in atkPlayerList) {
                player.GetComponent<PlayerMovement>().swapSides();
            }
            createPlayerLists();
    }  

    public void resetMap()
    {
        //Debug.Log("reset map called");
        Destroy(GameObject.Find("Grid"));
        Destroy(GameObject.Find("Grid(Clone)"));
        Destroy(GameObject.Find("Defuser(Clone)"));
        
        if (view.IsMine)
        {
            //Debug.Log("resetting tilemap called");
            PhotonNetwork.Instantiate(tileMap.name, new Vector3 (0, 0, 0), quaternion.identity);
        }
    }

    public void endGame(bool defWin)
    {
        // return to lobby
        Debug.Log("game over");
        foreach (GameObject player in defPlayerList) {
            Debug.Log("trying to call round end screen for def");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().gameEnd(defWin);
            }
        }
        foreach (GameObject player in atkPlayerList) {
            Debug.Log("trying to call round end screen for atk");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().gameEnd(!defWin);
            }
        }
    }
}
