using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

public class GrenadeScript : MonoBehaviour, ISecondaryGadget
{
    [SerializeField] private float dmgRadius;
    [SerializeField] private int maxDamage;

    private GameObject grenadeInstance;
    private Collider2D[] hitColliders;
    private ContactFilter2D noFilter;
    SecondaryGadgetScript throwingScript;
    PlayerMovement player;

    public void explode()
    {
        Debug.Log("grenade pos " + grenadeInstance.transform.position);
        hitColliders = new Collider2D[10];

        //get all gameobjects in each radius
        Physics2D.OverlapCircle(grenadeInstance.transform.position, dmgRadius, noFilter.NoFilter(), hitColliders);
        Debug.Log("hit collider length " + dmgRadius + " : " + hitColliders.Count());

        foreach(var hitCollider in hitColliders)
        {
            if (hitCollider && hitCollider.CompareTag("Player"))
            {
                player = hitCollider.transform.parent.parent.GetComponent<PlayerMovement>();    

                Debug.Log("found player");
                var closestPoint = hitCollider.ClosestPoint(grenadeInstance.transform.position);
                var distance = Vector3.Distance(closestPoint, grenadeInstance.transform.position);;
                var dmgPercent = Mathf.InverseLerp(dmgRadius, 0, distance);
                Debug.Log(dmgPercent + " = dmg%");
                //player.takeDmg((int)(maxDamage * dmgPercent));
                //view.RPC(nameof(doDamage), RpcTarget.All, dmgPercent);
                throwingScript.dealDamageHelper(dmgPercent, maxDamage, player.myID);
            }
        }

        DestroyImmediate(grenadeInstance, true);
    }



    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.GetChild(2).Find("FirePoint");
        throwingScript = player.GetComponent<SecondaryGadgetScript>();

        grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        grenadeInstance.GetComponent<Rigidbody2D>().AddForce(firePoint.up * force, ForceMode2D.Impulse);
        
        Debug.Log("player pos " + player.transform.position);
        Invoke("explode", force);
    }

    public string getName()
    {
        return "grenade";
    }
    public Sprite GetSprite() {
        return GetComponent<SpriteRenderer>().sprite;
    }
}
