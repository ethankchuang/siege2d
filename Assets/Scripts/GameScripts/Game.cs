using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;


public class Game : MonoBehaviour
{
    //public List<GameObject> defPlayerList;
    //public List<GameObject> atkPlayerList;
    public List<GameObject> defAlive;
    public List<GameObject> atkAlive;
    private int defPoints;
    private int atkPoints;
    [SerializeField] int PointsToWin;
    private PhotonView view;

    [SerializeField] private GameObject winRoundBG;
    [SerializeField] private GameObject loseRoundBG;
    [SerializeField] private GameObject winGameBG;
    [SerializeField] private GameObject loseGameBG;



    public void Start()
    {
        //defPlayerList = new List<GameObject>();
        //atkPlayerList = new List<GameObject>();
        defAlive = new List<GameObject>();
        atkAlive = new List<GameObject>();
        defPoints = 0;
        atkPoints = 0;
        view = GetComponent<PhotonView>();
    }

    /*public void addToList(GameObject player, bool isDef)
    {
        Debug.Log("add to list called");
        if (isDef)
        {
            defPlayerList.Add(player);
        }
        else
        {
            atkPlayerList.Add(player);
        }
    }*/

    public void createPlayerLists() 
    {
        foreach (GameObject player in FindObjectsOfType<GameObject>())
        {
            Debug.Log("gameobject is player? " + (player.GetComponent<PlayerMovement>() != null));
            if (player.GetComponent<PlayerMovement>() != null)
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
        //prepActiveList();
    }

    /*public void prepActiveList()
    {
        Debug.Log("prepping list");
        Debug.Log("defPlayerList " + defPlayerList.Count);
        Debug.Log("attackPlayerList " + atkPlayerList.Count);
        defAlive = defPlayerList;
        atkAlive = atkPlayerList;
        Debug.Log("defAlive " + defAlive.Count);
        Debug.Log("atkAlive " + atkAlive.Count);
        Debug.Log((defAlive == defPlayerList) + " def lists =?");
        Debug.Log((atkAlive == atkPlayerList) + " atk lists =?");
    }*/

    /*public bool removeFromList(GameObject player, bool isDef)
    {
        Debug.Log("removing from list");
        if (isDef)
        {
            defAlive.Remove(player);
            if (defAlive.Count <= 0)
            {
                //view.RPC(nameof(endRound), RpcTarget.All, false);
                //endRound(false);

                atkPoints ++;
                if (atkPoints >= PointsToWin)
                {
                    endGame(false);
                }
                else
                {
                    // what happens when u win the round 
                }
                return true;
            }
        }
        else
        {
            atkAlive.Remove(player);
            if (atkAlive.Count <= 0)
            {
                //view.RPC(nameof(endRound), RpcTarget.All, true);
                //endRound(true);


                defPoints ++;
                if (defPoints >= PointsToWin)
                {
                    endGame(true);
                }
                else
                {
                    // what happens when u win the round
                }
                return true;
            }
        }
        return false;
    }*/

    //[PunRPC]
    /*public void endRound(bool defWin)
    {
        if (defWin) 
        {
            defPoints ++;
            if (defPoints >= PointsToWin)
            {
                endGame(true);
            }
        }
        else
        {
            atkPoints ++;
            if (atkPoints >= PointsToWin)
            {
                endGame(false);
            }
        }
    }*/

    public void endGame(bool defWin)
    {

    }
}
