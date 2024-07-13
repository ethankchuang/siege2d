using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FlareScript : MonoBehaviour, ISecondaryGadget
{
    private GameObject grenadeInstance;

    public void explode()
    {
        DestroyImmediate(grenadeInstance, true);
    }


    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.Find("FirePoint");

        grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        grenadeInstance.GetComponent<Rigidbody2D>().AddForce(firePoint.up * force, ForceMode2D.Impulse);
        
        Debug.Log("player pos " + player.transform.position);
        Invoke("explode", force + 10);
    }

    public string getName()
    {
        return "flare";
    }
}
