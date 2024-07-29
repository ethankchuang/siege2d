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
    private GameObject currentSpawnPoint;

    private List<GameObject> defSpawnPoints;
    private List<GameObject> atkSpawnPoints;

    private List<GameObject> usedDefSpawnPoints;
    private List<GameObject> usedAtkSpawnPoints;

    [SerializeField] GameObject defPointParent;
    [SerializeField] GameObject atkPointParent;

    [SerializeField] Game game;


    private void Start()
    {
        Debug.Log("spawn players has been run and player count is " + playerCount);

        defSpawnPoints = new List<GameObject>();
        atkSpawnPoints = new List<GameObject>();
        usedDefSpawnPoints = new List<GameObject>();
        usedAtkSpawnPoints = new List<GameObject>();

        for (int i = 0; i < defPointParent.transform.childCount; i ++)
        {
            defSpawnPoints.Add(defPointParent.transform.GetChild(i).gameObject); 
        }
        for (int i = 0; i < atkPointParent.transform.childCount; i ++)
        {
            atkSpawnPoints.Add(atkPointParent.transform.GetChild(i).gameObject); 
        }        

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDef"])
        {
            currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];

            while (usedDefSpawnPoints.Contains(currentSpawnPoint))
            {
                currentSpawnPoint = defSpawnPoints[Random.Range(0, defSpawnPoints.Count)];
            }
            
            pos = currentSpawnPoint.transform.position;
            usedDefSpawnPoints.Add(currentSpawnPoint);

            //PhotonNetwork.Instantiate(defenderPrefab.name, pos, Quaternion.identity);
            game.addToList(PhotonNetwork.Instantiate(defenderPrefab.name, pos, Quaternion.identity), true);

            // FOR TESTING ONLY
            game.prepActiveList();
            Debug.Log("player 1 (defender) joined the game");
        }
        else
        {
            currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];

            while (usedAtkSpawnPoints.Contains(currentSpawnPoint))
            {
                currentSpawnPoint = atkSpawnPoints[Random.Range(0, atkSpawnPoints.Count)];
            }
            
            pos = currentSpawnPoint.transform.position;
            usedAtkSpawnPoints.Add(currentSpawnPoint);

            //PhotonNetwork.Instantiate(attackerPrefab.name, pos, Quaternion.identity);
            game.addToList(PhotonNetwork.Instantiate(attackerPrefab.name, pos, Quaternion.identity), false);

            // FOR TESTING ONLY
            game.prepActiveList();
            Debug.Log("player 2 (attacker) joined the game");
        }

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }
}
