using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashbangScript : MonoBehaviour, ISecondaryGadget
{
    [SerializeField] private float dmgRadius;
    [SerializeField] private float maxBrightness;
    private Collider2D[] hitColliders;
    private ContactFilter2D noFilter;
    SecondaryGadgetScript throwingScript;

    public void explode()
    {
        hitColliders = new Collider2D[20];
        Physics2D.OverlapCircle(transform.position, dmgRadius, noFilter.NoFilter(), hitColliders);
        foreach(var hitCollider in hitColliders)
        {
            if (hitCollider && hitCollider.CompareTag("Player"))
            {
                var player = hitCollider.transform.parent.parent.GetComponent<PlayerMovement>();    
                //Debug.Log((player == null) + " player is null?");
                Debug.Log(" player ID " + player.myID);
                var closestPoint = hitCollider.ClosestPoint(transform.position);
                var distance = Vector3.Distance(closestPoint, transform.position);
                float percent = Mathf.InverseLerp(dmgRadius, 0, distance);
                throwingScript.setLightHelper(percent, maxBrightness, player.myID);
            }
        }
        playSound();
        Destroy(gameObject);
    }
    public String getName()
    {
        return "flashbang";
    }

    public void throwGadget(float force, GameObject player)
    {
        Transform firePoint = player.transform.GetChild(2).Find("FirePoint");
        //grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        gameObject.GetComponent<Rigidbody2D>().AddForce(firePoint.up * force, ForceMode2D.Impulse);
        throwingScript = player.GetComponent<SecondaryGadgetScript>();
        Invoke(nameof(explode), force);
    }
    public Sprite GetSprite() {
        return GetComponent<SpriteRenderer>().sprite;
    }
    
    public void playSound() {
        Debug.Log("play sound called");
        transform.GetChild(1).gameObject.SetActive(true);
    }
}
