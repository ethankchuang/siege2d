using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
//using UnityEditorInternal;

public class PlaceDefuser : MonoBehaviour
{
    public GameObject defuser;
    public Transform placementPoint;
    public float placementTime;
    PlayerMovement playerMovement;
    //DefuserScript defuserScript;

    //true for testing only
    public bool hasDefuser = true;

    PhotonView view;
    Tilemap tilemap;
    [SerializeField] Tile bombSiteTile;

    public void Start()
    {
        view = GetComponent<PhotonView>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        //defuserScript = GameObject.Find("Game").GetComponent<DefuserScript>();
    }

    void Update()
    {
        if (view.IsMine)
        {
            var tileMapGO = GameObject.Find("Grid");
            if (tileMapGO == null) {
                tilemap = GameObject.Find("Grid(Clone)").transform.Find("Floor").GetComponent<Tilemap>();
            } else {
                tilemap = GameObject.Find("Grid").transform.Find("Floor").GetComponent<Tilemap>();
            }
            
            if (Input.GetKeyDown("f") && hasDefuser && tilemap.GetTile(tilemap.WorldToCell(placementPoint.position)) == bombSiteTile ) 
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
        //Instantiate(defuser, placementPoint.position, transform.rotation);
        GameObject defuserParent = GameObject.Find("DefuserParent");
        defuserParent.transform.position = placementPoint.position;
        defuserParent.transform.GetChild(0).gameObject.SetActive(true);
        defuserParent.transform.rotation = transform.rotation;
        Debug.Log("defuserparent location " + defuserParent.transform.position);
        Debug.Log("placement point location " + placementPoint.transform.position);
        //Debug.Log("defuserparent location " + defuserParent.transform.position);
        GameObject.Find("Canvas").GetComponent<NewTimer>().defuserPlaced();

        Debug.Log("place defuser helper called");
        //defuserScript.defuserPlaced();
        //defuseTime.GetComponent<NewTimer>().defuserPlaced();
    } 
}
