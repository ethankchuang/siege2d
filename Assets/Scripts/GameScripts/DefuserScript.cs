using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class DefuserScript : MonoBehaviour
{
    [SerializeField] private float disarmTime;
    private float currentTime;
    bool beingDefused = false;
    GameObject defuser;
    PhotonView view;
    GameObject playerDefusing;
    void Start()
    {
        view = GetComponent<PhotonView>();
        currentTime = disarmTime;
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
        currentTime -= Time.deltaTime;
        view.RPC("disarmingHelper", RpcTarget.All, currentTime);
        playerDefusing = pDefusing;
    }

    [PunRPC]
    public void disarmingHelper(float curTime)
    {
        currentTime = curTime;
        
        Debug.Log("disarm time = " + currentTime);

        if (currentTime <= 0)
        {
            // implement later
            Debug.Log("bomb defused, defense wins");
            
            currentTime = disarmTime;
            beingDefused = false;
            Game game = GameObject.Find("Game").GetComponent<Game>();
            game.endRoundHelper(true);
            //gameObject.SetActive(false); 
        }

        beingDefused = true;
    }
    

    /*public void defuserPlaced()
    {
        Debug.Log("defuser is defined in defuser script");
        defuser = GameObject.Find("Defuser(Clone)");
    }

    public GameObject getDefuser()
    {
        Debug.Log("get defuser has been called");
        return defuser;
    }*/
}
