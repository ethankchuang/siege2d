using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;
using Unity.VisualScripting;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using System;

public class SoftWallScript : MonoBehaviour
{
    public Tilemap tilemap;
    PhotonView view;
    private ShadowCaster2DCreator shadowCaster2DCreator;
    bool wasDestroyed;
    int counter;
    BoundsInt bounds;
    List<UnityEngine.Vector3Int> allTilesLocations;
    //GameObject center;

    void Start()
    {
        view = GetComponent<PhotonView>();
        tilemap = GetComponent<Tilemap>();
        shadowCaster2DCreator = GetComponent<ShadowCaster2DCreator>();
        wasDestroyed = false;
        counter = 5;


        Vector3Int loc;
        allTilesLocations = new List<UnityEngine.Vector3Int>();
        bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x ++) {
            for (int y = bounds.yMin; y < bounds.yMax; y ++) {
                loc = new Vector3Int(x, y, 0);

                if (tilemap.HasTile(loc))
                {
                    //Debug.Log("allTilesLocations added at cell: " + loc);
                    //Debug.Log("allTilesLocations added at world:" + tilemap.CellToWorld(loc));
                    //allTilesLocations.Add(tilemap.CellToWorld(loc));
                    allTilesLocations.Add(loc);
                }
            }
        }
    }

    public void RecieveHitRaycast(RaycastHit2D hit, double x, double y)
    {
        //TileBase hitTile = tilemap.GetTile(tilemap.WorldToCell(new Vector2(hit.point.x - (hit.normal.x - 0.01f), hit.point.y - (hit.normal.y * 0.01f))));

        /*Vector3Int tileCord = tilemap.WorldToCell(hit);
        Debug.Log("tile at current pos? before");
        Debug.Log(tilemap.GetTile(tileCord) == null);

        double xDif = x - tileCord.x;
        double yDif = y - tileCord.y;   
        Debug.Log("my x " + x + " my y " + y + " xDif " + xDif + " yDif " + yDif);
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
        Debug.Log("tile at current pos? after");
        Debug.Log(tilemap.GetTile(tileCord) == null);*/

        //Debug.Log("soft wall recieve hit");
        var cellPos = tilemap.WorldToCell(new UnityEngine.Vector3(hit.point.x - (hit.normal.x * 0.01f), hit.point.y - (hit.normal.y * 0.01f)));
        view.RPC("takeDmg", RpcTarget.All, cellPos.x, cellPos.y);
    }

    [PunRPC]
    public void takeDmg(int x, int y)
    {
        //Debug.Log("take dmg called, has tile? " + (tilemap.GetTile(new Vector3Int (x, y)) != null)); 
        Vector3Int tilePos = new Vector3Int(x, y);//tilemap.WorldToCell(new Vector2 (x, y));
        //Debug.Log("tile pos " + tilePos.x + ", " + tilePos.y);
        tilemap.SetTile(tilePos, null);
        //Debug.Log("removing tile, has tile? " + (tilemap.GetTile(new Vector3Int (x, y)) == null)); 
        wasDestroyed = true;
    }

    void Update()
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

    public void RecieveHitAOE(GameObject center, float radius)
    {
        Debug.Log("receive hit AOE called");
        for (int i = 0; i < allTilesLocations.Count; i ++)
        {
            //Debug.Log(allTilesLocations[i] + " <- all tile locations, grenade pos -> " + center);
            var distance = Vector2.Distance(tilemap.CellToWorld(allTilesLocations[i]), center.transform.position);
            //Debug.Log(distance + " distance between grenade and tile " + i);
            if (distance <= radius)
            {
                view.RPC("takeDmg", RpcTarget.All, allTilesLocations[i].x, allTilesLocations[i].y);
                Debug.Log("called recieve hit");

                /*Vector3Int tilePos = tilemap.WorldToCell(new Vector2 (allTilesLocations[i].x, allTilesLocations[i].y));

                tilemap.SetTile(tilePos, null);
                wasDestroyed = true;

                var tm = new ArrayList();
                for (int x = bounds.xMin; x < bounds.xMax; x ++) {
                    for (int y = bounds.yMin; y < bounds.yMax; y ++) {
                        var loc = new Vector3Int(x, y, 0);
                        tm.Add((loc.x, loc.y));
                    }
                }
                view.RPC(nameof(setTilemaps), RpcTarget.All, tm);*/
            }
        }
    }
    /*public void RecieveHitAOEHelper(GameObject gameObject, float radius) {
        center = gameObject;
        view.RPC(nameof(RecieveHitAOE), RpcTarget.All, radius);
    }*/
    /*[PunRPC]
    public void setTilemaps(ArrayList tmArray) {
        tilemap.ClearAllTiles();
        for (int i = 0; i < tmArray.Count; i ++) {
            tilemap.
        }
    }*/
}
