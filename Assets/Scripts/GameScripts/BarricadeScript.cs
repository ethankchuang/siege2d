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

public class BarricadeScript : MonoBehaviour
{
    //public Tile barricade;
    public Tilemap tilemap;
    public int health = 8;

    Dictionary<Vector3Int, int> dict = new Dictionary<Vector3Int, int>();
    PhotonView view;


    public void Start()
    {
        view = GetComponent<PhotonView>();
        //Debug.Log("setting view:");
        //Debug.Log( view); ////// <= null here already
        tilemap = GetComponent<Tilemap>();
    }

    public void RecieveHit(UnityEngine.Vector2 hit, double x, double y)
    {
        //Debug.Log("barricade recieved hit" + hit);
        //Debug.Log(hit);
        view.RPC("TakeDamage", RpcTarget.All, hit, x, y);
    }

    [PunRPC]
    public void TakeDamage(UnityEngine.Vector2 hit, double x, double y)
    {
        Vector3Int tileCord = tilemap.WorldToCell(hit);
        //Debug.Log("tile at" + tileCord);

        double xDif = x - tileCord.x;
        double yDif = y - tileCord.y;
        //Debug.Log("my x " + x + " my y " + y + " xDif " + xDif + " yDif " + yDif);
        if (tilemap.GetTile(tileCord) == null && xDif > yDif && xDif > 0)
        {
            tileCord.x -= 1;
        }
        else if (tilemap.GetTile(tileCord) == null && yDif > xDif && yDif > 0)
        {
            tileCord.y -= 1;
        }

        //Debug.Log(tilemap.GetTile(tileCord));
        //Debug.Log("now tile at" + tileCord);

        if (!dict.ContainsKey(tileCord))
        {
            dict.Add(tileCord, health - 1);
            Debug.Log(dict[tileCord]);
        }
        else
        {
            dict[tileCord] -= 1;
            Debug.Log(dict[tileCord]);
            if (dict[tileCord] == 0)
            {
                tilemap.SetTile(tileCord, null);
            }
        }
        view.RPC("TakeDamageHelper", RpcTarget.All, tileCord.x, tileCord.y, "none");
    }

    [PunRPC]
    public void TakeDamageHelper(int curX, int curY, String prev)
    {
        Vector3Int current = new Vector3Int(curX, curY, 0);

        if (tilemap.GetTile(current) == null && prev != "none") return;

        if (prev == "right" || prev == "none") view.RPC("TakeDamageHelper", RpcTarget.All, curX + 1, curY, "right");
        if (prev == "left" || prev == "none") view.RPC("TakeDamageHelper", RpcTarget.All, curX - 1, curY, "left");
        if (prev == "top" || prev == "none") view.RPC("TakeDamageHelper", RpcTarget.All, curX, curY + 1, "top");
        if (prev == "bottom" || prev == "none")view.RPC("TakeDamageHelper", RpcTarget.All, curX, curY - 1, "bottom");

        if (prev == "none") return;

        if (!dict.ContainsKey(current))
        {
            dict.Add(current, health - 1);
            Debug.Log(dict[current]);
        }
        else
        {
            dict[current] -= 1;
            Debug.Log(dict[current]);
            if (dict[current] == 0)
            {
                tilemap.SetTile(current, null);
            }
        }

    }
}