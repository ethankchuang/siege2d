using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine.Tilemaps;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class BarricadeScript : MonoBehaviour
{
    //public Tile barricade;
    public Tilemap tilemap;
    public int health = 8;

    Dictionary<Vector3Int, int> dict = new Dictionary<Vector3Int, int>();
    PhotonView view;
    private List<GameObject> myShadowCasters = new List<GameObject>();

    private ShadowCaster2DCreator shadowCaster2DCreator;
    private bool wasDestroyed;
    private int counter;


    [PunRPC]
    public void CreateHelper()
    {
        shadowCaster2DCreator.DestroyBarricade();
        wasDestroyed = false;   
    }

    public void Update()
    {
        if (wasDestroyed)
        {
            counter --;
            if (counter <= 0)
            {
                shadowCaster2DCreator.Create();
                wasDestroyed = false;   
                //view.RPC("CreateHelper", RpcTarget.All);
                counter = 5;
            }
        }
    }
    public void Start()
    {
        view = GetComponent<PhotonView>();
        tilemap = GetComponent<Tilemap>();
        shadowCaster2DCreator = GetComponent<ShadowCaster2DCreator>();
        counter = 5;
        

        for (int i = 0; i < transform.childCount; i ++)
        {
            myShadowCasters.Add(transform.GetChild(i).gameObject);
        }
    }

    public void RecieveHit(UnityEngine.Vector2 hit, double x, double y)
    {
        Vector3Int tileCord = tilemap.WorldToCell(hit);
        //Debug.Log("tile at" + tileCord);

 
        double xDif = x - tileCord.x;
        double yDif = y - tileCord.y;
        //Debug.Log("my x " + x + " my y " + y + " xDif " + xDif + " yDif " + yDif);
        if (tilemap.GetTile(tileCord) == null && xDif > yDif && xDif > 0)
        {
            tileCord.x -= 1;
            if (tilemap.GetTile(tileCord) == null)
            {
                tileCord.x += 1;
                tileCord.y -= 1;
            }
        }
        else if (tilemap.GetTile(tileCord) == null && yDif > xDif && yDif > 0)
        {
            tileCord.y -= 1;
        }
        //Debug.Log("now tile at" + tileCord);

        //view.RPC("TakeDamageHelper", RpcTarget.All, tileCord.x, tileCord.y, "none");
        TakeDamageHelper(tileCord.x, tileCord.y, "none");



        //view.RPC("TakeDamage", RpcTarget.All, tileCord.x, tileCord.y );
        
    }

    [PunRPC]
    public void TakeDamage(int x, int y)
    {
        //Debug.Log(tilemap.GetTile(tileCord));
        //Debug.Log("now tile at" + tileCord);
        Vector3Int tileCord = new Vector3Int(x, y, 0);

        if (!dict.ContainsKey(tileCord))
        {
            dict.Add(tileCord, health - 1);
            //Debug.Log(dict[tileCord]);
        }
        else
        {
            dict[tileCord] -= 1;
            //Debug.Log(dict[tileCord]);
            if (dict[tileCord] == 0)
            {
                //TODO NEED TO FIX / DO
                tilemap.SetTile(tileCord, null);
                //Debug.Log("barricade tile cord: " + tileCord);
                //shadowCaster2DCreator.DestroyOldShadowCasters();
                //StartCoroutine(waiter());
                wasDestroyed = true;
                //shadowCaster2DCreator.Create();
                
                //Destroy(shadowCaster2D);
            }
        }
    }

    public void TakeDamageHelper(int curX, int curY, String prev)
    {
        Vector3Int current = new Vector3Int(curX, curY, 0);

        if (tilemap.GetTile(current) == null && prev != "none") return;

        if (prev == "right" || prev == "none") TakeDamageHelper(curX + 1, curY, "right");
        if (prev == "left" || prev == "none") TakeDamageHelper(curX - 1, curY, "left");
        if (prev == "top" || prev == "none") TakeDamageHelper(curX, curY + 1, "top");
        if (prev == "bottom" || prev == "none") TakeDamageHelper(curX, curY - 1, "bottom");

        view.RPC("TakeDamage", RpcTarget.All, curX, curY);

        //if (prev == "none") return;
    }
}