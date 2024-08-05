using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DefuserScript : MonoBehaviour
{
    [SerializeField] private float disarmTime;
    bool beingDefused = false;
    GameObject defuser;
    PhotonView view;
    GameObject playerDefusing;
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public bool getBeingDisarmed()
    {
        return beingDefused;
    }

    public void stopDisarming()
    {
        beingDefused = false;
        playerDefusing = null;
    }

    public GameObject beingDisarmedBy()
    {
        return playerDefusing;
    }

    public void disarming(GameObject pDefusing)
    {
        view.RPC("disarmingHelper", RpcTarget.All);
        playerDefusing = pDefusing;
    }

    [PunRPC]
    public void disarmingHelper()
    {
        disarmTime -= Time.deltaTime;
        Debug.Log("disarm time = " + disarmTime);

        if (disarmTime <= 0)
        {
            // implement later
            Debug.Log("bomb defused, defense wins");
            Game game = GameObject.Find("Game").GetComponent<Game>();
            game.endRound(true);
        }

        beingDefused = true;
    }

    public void defuserPlaced()
    {
        Debug.Log("defuser is defined in defuser script");
        defuser = GameObject.Find("Defuser(Clone)");
    }

    public GameObject getDefuser()
    {
        Debug.Log("get defuser has been called");
        return defuser;
    }
}
