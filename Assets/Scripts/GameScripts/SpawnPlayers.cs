using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Security;
using UnityEngine.Rendering.Universal;
using ExitGames.Client.Photon.StructWrapping;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject defenderPrefab;
    public GameObject attackerPrefab;
    //public GameObject camHolder;
    public int playerCount = 0;
    ExitGames.Client.Photon.Hashtable CustomeValue;
    public Vector2 pos;
    private Vector2 currentSpawnPoint;

    private List<Vector2> defSpawnPoints;
    private List<Vector2> atkSpawnPoints;

    private List<Vector2> usedDefSpawnPoints;
    private List<Vector2> usedAtkSpawnPoints;

    [SerializeField] GameObject defPointParent;
    [SerializeField] GameObject atkPointParent;

    [SerializeField] Game game;
    PhotonView view;


    private void Start()
    {
        //Debug.Log("spawn players has been run and player count is " + playerCount);
        view = GetComponent<PhotonView>();

        // need rpc so spawn points dont overlap?
        defSpawnPoints = new List<Vector2>();
        atkSpawnPoints = new List<Vector2>();
        usedDefSpawnPoints = new List<Vector2>();
        usedAtkSpawnPoints = new List<Vector2>();

        for (int i = 0; i < defPointParent.transform.childCount; i ++) {
            defSpawnPoints.Add(defPointParent.transform.GetChild(i).transform.position); 
        }
        for (int i = 0; i < atkPointParent.transform.childCount; i ++) {
            atkSpawnPoints.Add(atkPointParent.transform.GetChild(i).transform.position); 
        }        

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDef"]) {
            currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];
            while (usedDefSpawnPoints.Contains(currentSpawnPoint)) {
                currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];
            }
            pos = currentSpawnPoint;
            PhotonNetwork.Instantiate(defenderPrefab.name, pos, Quaternion.identity);
        } else {
            currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];
            while (usedAtkSpawnPoints.Contains(currentSpawnPoint)) {
                currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];
            }
            pos = currentSpawnPoint;
            PhotonNetwork.Instantiate(attackerPrefab.name, pos, Quaternion.identity);
        }
        view.RPC(nameof(takeOffList), RpcTarget.All, pos.x, pos.y, (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDef"]);

        //Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    [PunRPC]
    public void takeOffList(float x, float y, bool isDef) {
        
        if (isDef) {
            usedDefSpawnPoints.Add(new Vector2(x, y));
        } else {
            usedAtkSpawnPoints.Add(new Vector2(x, y));
        }
    }

    public void resetUsedLists() {
        usedAtkSpawnPoints.Clear();
        usedDefSpawnPoints.Clear();
    }
    public Vector2 respawn(GameObject player)
    {
        if (player.GetComponent<PlayerMovement>().isDef) {
            currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];
            while (usedDefSpawnPoints.Contains(currentSpawnPoint)) {
                currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];
            }
        } else {
            currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];
            while (usedAtkSpawnPoints.Contains(currentSpawnPoint)) {
                currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];
            }
        }
        pos = currentSpawnPoint;
        view.RPC(nameof(takeOffList), RpcTarget.All, pos.x, pos.y, player.GetComponent<PlayerMovement>().isDef);

        return pos;
    }
}
