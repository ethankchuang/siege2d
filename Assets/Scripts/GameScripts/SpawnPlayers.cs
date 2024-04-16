using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    //public GameObject camHolder;
    public int playerCount = 0;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public Vector2 pos;

    private void Start()
    {
        if (playerCount == 0)
        {
            pos = new Vector2(0, 0);
        }
        else
        {
            pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
        playerCount ++;
    }
}
