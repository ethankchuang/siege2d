using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;


public class Game : MonoBehaviour
{
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
                if(player.GetComponent<PlaceDefuser>() != null)
                {
                    //atkPlayerList.Add(player);
                    atkAlive.Add(player);
                }
                else if (player.GetComponent<DisarmDefuser>() != null)
                {
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
                if(player.GetComponent<PlaceDefuser>() != null)
                {
                    //atkPlayerList.Add(player);
                    atkPlayerList.Add(player);
                }
                else if (player.GetComponent<DisarmDefuser>() != null)
                {
                    //defPlayerList.Add(player);
                    defPlayerList.Add(player);
                }
            }
        }
        //Debug.Log("def count " + defPlayerList.Count);
        //Debug.Log("atk count " + atkPlayerList.Count);
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

        //Debug.Log("def count " + defPlayerList.Count);
        //Debug.Log("atk count " + atkPlayerList.Count);
        foreach (GameObject player in defPlayerList) {
            Debug.Log("trying to call round end screen for def");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().roundEndScreen(defWin);
            }
        }
        foreach (GameObject player in atkPlayerList) {
            Debug.Log("trying to call round end screen for atk");
            if (player.GetComponent<PlayerMovement>().nickName == PhotonNetwork.LocalPlayer.NickName) {
                player.GetComponent<PlayerMovement>().roundEndScreen(!defWin);
            }
        }

    } 

    public void resetMap()
    {
        //Debug.Log("reset map called");
        Destroy(GameObject.Find("Grid"));
        Destroy(GameObject.Find("Grid(Clone)"));
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
