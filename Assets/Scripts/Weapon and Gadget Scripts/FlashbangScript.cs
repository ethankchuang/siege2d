using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashbangScript : MonoBehaviour, ISecondaryGadget
{
    [SerializeField] private float dmgRadius;
    [SerializeField] private int maxBrightness;

    private GameObject grenadeInstance;
    private Collider2D[] hitColliders;
    private ContactFilter2D noFilter;
    private Light2D targetLight;

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
                var player = hitCollider.GetComponent<PlayerMovement>();    

                Debug.Log("found player");
                var closestPoint = hitCollider.ClosestPoint(grenadeInstance.transform.position);
                var distance = Vector3.Distance(closestPoint, grenadeInstance.transform.position);;
                var percent = Mathf.InverseLerp(dmgRadius, 0, distance);

                targetLight = player.gameObject.transform.Find("Spot Light 2D").GetComponent<Light2D>();
                targetLight.intensity = maxBrightness * percent;
                
                //targetLight.pointLightOuterAngle = 360;
                //targetLight.pointLightInnerAngle = 180;
            }
        }

        Destroy(grenadeInstance);
    }

    public String getName()
    {
        return "flashbang";
    }

    public void throwGadget(float force, GameObject player)
    {

        Transform firePoint = player.transform.Find("FirePoint");

        grenadeInstance = Instantiate(gameObject, firePoint.position, firePoint.rotation);
        //grenadeInstance = PhotonNetwork.Instantiate(gameObject.name, firePoint.position, firePoint.rotation);
        grenadeInstance.GetComponent<Rigidbody2D>().AddForce(firePoint.up * force, ForceMode2D.Impulse);
        
        Debug.Log("player pos " + player.transform.position);
        Invoke(nameof(explode), force);
    }

    /*public void explodeHelper()
    {
        view = PhotonView.Get(this);
        if (view.IsMine)
        {
            Debug.Log("explode helper called");
            view.RPC("explode", RpcTarget.All);
        }
    }

    public void Start()
    {
        view = GetComponent<PhotonView>();
    }*/
}
