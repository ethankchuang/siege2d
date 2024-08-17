using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class BreachChargeScript : MonoBehaviour, ISecondaryGadget
{
    [SerializeField] private float dmgRadius;
    private GameObject grenadeInstance;
    private Collider2D[] hitColliders;
    private ContactFilter2D noFilter;
    bool isMine = false;
    SoftWallScript softWallScript;
    PhotonView view;


    public void explode()
    {
        //if () {
        //Debug.Log("explode called for masta client");
            //hitColliders = new Collider2D[30]; 

            //get all gameobjects in each radius
            //Physics2D.OverlapCircle(transform.position, dmgRadius, noFilter.NoFilter(), hitColliders);
            //Debug.Log("hit collider length " + dmgRadius + " : " + hitColliders.Count());
            //foreach(var hitCollider in hitColliders)
            //{  
            //    if (hitCollider && hitCollider.GetComponent<SoftWallScript>())
            //    {
            //        hitCollider.GetComponent<SoftWallScript>().RecieveHitAOE(gameObject, dmgRadius); 
            //        //grenadeInstance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; 
            //    }
            // }
            //Debug.Log("DestroyImmediate");
            //gameObject.SetActive(false); 
            
        //}
        softWallScript.RecieveHitAOE(gameObject, dmgRadius);
        view = GetComponent<PhotonView>();
        view.RPC(nameof(destroyGO), RpcTarget.All);
    }
    [PunRPC]
    public void destroyGO() {
        Destroy(gameObject);
    }


    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.GetChild(2).Find("FirePoint");
        //Debug.Log("throw gadget called, force = " + force);
        isMine = true;

        //grenadeInstance = PhotonNetwork.Instantiate(gameObject.name, firePoint.position, firePoint.rotation);
        //grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        gameObject.GetComponent<Rigidbody2D>().AddForce(firePoint.up * (force + 5), ForceMode2D.Impulse);
        
        //Debug.Log("grenade pos immediate velocity " + grenadeInstance.GetComponent<Rigidbody2D>().velocity);
        //view = player.GetComponent<PhotonView>();

    }

    public string getName()
    {
        return "BreachCharge";
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {   
        //Debug.Log("collision occured");
        if (collision.gameObject.GetComponent<SoftWallScript>() && isMine) {
            softWallScript = collision.gameObject.GetComponent<SoftWallScript>();
            explode();
        } else if (isMine) {
            view = GetComponent<PhotonView>();
            view.RPC(nameof(destroyGO), RpcTarget.All);
        }
    }
    public Sprite GetSprite() {
        return GetComponent<SpriteRenderer>().sprite;
    }
}
