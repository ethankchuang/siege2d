using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityEditorInternal;

public class PlaceDefuser : MonoBehaviour
{
    public GameObject defuser;
    public Transform placementPoint;
    public float placementTime;
    PlayerMovement playerMovement;
    DefuserScript defuserScript;

    //true for testing only
    public bool hasDefuser = true;

    PhotonView view;

    public void Start()
    {
        view = GetComponent<PhotonView>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        defuserScript = GameObject.Find("Game").GetComponent<DefuserScript>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown("f") && hasDefuser) 
            {
                Invoke("placeDefuser", placementTime);
                hasDefuser = false;
                playerMovement.setPlayerLocker(true);
            }
        } 
    }

    public void placeDefuser()
    {
        playerMovement.setPlayerLocker(false);
        view.RPC("placeDefuserHelper", RpcTarget.All);
    }

    [PunRPC]
    public void placeDefuserHelper()
    {
        Instantiate(defuser, placementPoint.position, transform.rotation);


        GameObject.Find("Canvas").GetComponent<NewTimer>().defuserPlaced();

        Debug.Log("place defuser told defuser script that defuser was placed");
        defuserScript.defuserPlaced();
        //defuseTime.GetComponent<NewTimer>().defuserPlaced();
    } 
}
