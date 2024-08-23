using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FlareScript : MonoBehaviour, ISecondaryGadget
{
    public void explode()
    {
        DestroyImmediate(gameObject, true);
    }

    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.GetChild(2).Find("FirePoint");

        //grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        gameObject.GetComponent<Rigidbody2D>().AddForce(firePoint.up * force, ForceMode2D.Impulse);
        
        Debug.Log("player pos " + player.transform.position);
        Invoke("explode", force + 10);
    }

    public string getName()
    {
        return "flare";
    }
    public Sprite GetSprite() {
        return GetComponent<SpriteRenderer>().sprite;
    }
}
