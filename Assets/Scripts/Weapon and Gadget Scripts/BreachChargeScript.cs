using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BreachChargeScript : MonoBehaviour, ISecondaryGadget
{
    [SerializeField] private float dmgRadius;
    private GameObject grenadeInstance;
    private Collider2D[] hitColliders;
    private ContactFilter2D noFilter;

    public void explode()
    {

        Debug.Log("grenade pos " + transform.position);
        hitColliders = new Collider2D[10]; 

        //get all gameobjects in each radius
        Physics2D.OverlapCircle(transform.position, dmgRadius, noFilter.NoFilter(), hitColliders);
        Debug.Log("hit collider length " + dmgRadius + " : " + hitColliders.Count());
        foreach(var hitCollider in hitColliders)
        {  
            if (hitCollider && hitCollider.GetComponent<SoftWallScript>())
            {
                hitCollider.GetComponent<SoftWallScript>().RecieveHitAOE(gameObject, dmgRadius); 
                //grenadeInstance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; 
            }
        }

        Debug.Log("DestroyImmediate");
        gameObject.SetActive(false); 
    }


    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.Find("FirePoint");
        
        Debug.Log("throw gadget called, force = " + force);

        grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        grenadeInstance.GetComponent<Rigidbody2D>().AddForce(firePoint.up * (force + 5), ForceMode2D.Impulse);
        
        Debug.Log("grenade pos immediate velocity " + grenadeInstance.GetComponent<Rigidbody2D>().velocity);
    }

    public string getName()
    {
        return "breachCharge";
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {   
        Debug.Log("collision occured");
        if (collision.gameObject.GetComponent<SoftWallScript>())
        {
            explode();
        }
    }
}
