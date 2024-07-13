using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
//using Unity.VisualScripting.Dependencies.Sqlite;

public class DisarmDefuser : MonoBehaviour
{
    GameObject defuser;
    DefuserScript defuserScript;
    PlayerMovement playerMovement;

    private float disarmTime;
    private Boolean disarmLocked;

    bool defuserKnown;

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        defuserScript = GameObject.Find("Game").GetComponent<DefuserScript>();
        defuserKnown = false;
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKey("f"))
            {
                if (defuserKnown
                    && transform.position.x > defuser.transform.position.x - 10/*3.5*/
                    && transform.position.x < defuser.transform.position.x + 10/*3.5*/
                    && transform.position.y > defuser.transform.position.y - 10/*3.5*/
                    && transform.position.y < defuser.transform.position.y + 10/*3.5*/
                    && (defuserScript.beingDisarmedBy() == gameObject || !defuserScript.getBeingDisarmed()))
                {
                    Debug.Log("defusing defuser");
                    defuserScript.disarming(gameObject);
                    playerMovement.setPlayerDisarming(true);
                }
                else if (!defuserKnown)                                                        
                {
                    defuser = defuserScript.getDefuser();

                    if (defuser.IsType<GameObject>())
                    {
                        defuserKnown = true;
                    }
                    if (defuser == null)
                    {
                        Debug.Log("defuser is null");
                    }
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
