using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
//using Unity.VisualScripting.Dependencies.Sqlite;

public class DisarmDefuser : MonoBehaviour
{
    public GameObject defuser;
    public GameObject defuserParent;
    DefuserScript defuserScript;
    PlayerMovement playerMovement;
    bool defuserDown;

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        defuserParent = GameObject.Find("DefuserParent");
        defuserDown = false;
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (defuserParent.GetComponentInChildren<DefuserScript>() != null) {
                defuser = defuserParent.GetComponentInChildren<DefuserScript>().gameObject;
                defuserScript = defuserParent.GetComponentInChildren<DefuserScript>();
                defuserDown = true;
            } else { defuserDown = false;}
            //Debug.Log(defuserDown + " defuser down?");
            //Debug.Log(defuser.transform.position.x + ", " + defuser.transform.position.y + " defuser pos?");
            //Debug.Log(transform.position.x + ", " + transform.position.y + " my pos?");
            //Debug.Log(!defuserScript.getBeing Disarmed() + " being disarmed");
            //Debug.Log((defuserScript.beingDisarmedBy() == gameObject) + " being disarmed by me");
            

            if (Input.GetKey("f"))
            { 
                if (defuserDown
                    && transform.position.x > defuser.transform.position.x - 0.6/*3.5*/
                    && transform.position.x < defuser.transform.position.x + 0.6/*3.5*/
                    && transform.position.y > defuser.transform.position.y - 0.6/*3.5*/
                    && transform.position.y < defuser.transform.position.y + 0.6/*3.5*/
                    && (!defuserScript.getBeingDisarmed() || defuserScript.beingDisarmedBy() == gameObject)) {
                    Debug.Log("defusing defuser");
                    defuserScript.disarming(gameObject);
                    playerMovement.setPlayerDisarming(true);
                }
            }
            else if (Input.GetKeyUp("f") && playerMovement.isPlayerDisarming())
            {
                playerMovement.setPlayerDisarming(false);
                defuserScript.stopDisarming();
            }
        }
    }
}
