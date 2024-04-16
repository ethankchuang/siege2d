using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

public class BarricadeScript : MonoBehaviour, IShootAble
{
    public int health = 8;
    PhotonView view;
    public void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void RecieveHit(RaycastHit2D hit)
    {
        Debug.Log("recieved hit");
        view.RPC("TakeDamage", RpcTarget.All);
    }

    [PunRPC]
    public void TakeDamage()
    {
        Debug.Log("took damage");
        health -= 1;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}