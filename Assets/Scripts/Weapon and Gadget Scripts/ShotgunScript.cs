using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Mathematics;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;

public class ShotgunScript : MonoBehaviour, IWeaponScript  
{
    [SerializeField] public GameObject bulletTrail;
    [SerializeField]private float weaponRange;
    [SerializeField]private float adsRangeIncrease;
    [SerializeField]private float hipSpread;
    [SerializeField]private float adsSpread;
    [SerializeField]private int maxAmmo;
    private float currentAmmo;
    [SerializeField]private float timeBetweenShots;
    [SerializeField]private float reloadTime;
    [SerializeField]private int adsInnerAngle;
    [SerializeField]private int adsOuterAngle;
    [SerializeField]private int damage;
    [SerializeField]private AudioClip shotSoundClip;
    [SerializeField]private AudioClip reloadSoundClip;
    public Sprite sprite;
    bool firing = false;
    bool reloading = false;
    bool canShoot = true;
    bool isADS = false;
    Vector3 spr;
    private Transform muzzleFlash;
    PlayerMovement playerMovement;

    // for testing and funsies
    [SerializeField] private int numBullets;

    public void onStart()
    {
        currentAmmo = maxAmmo;
        //Debug.Log("on start called " + currentAmmo);
        reloading = false;
    }

    public bool shoot(Transform firePoint, AudioSource audioSource)
    {
        //Debug.Log("shoot called " + canShoot + " can shoot " + currentAmmo + " current ammo");
        //Debug.Log(firePoint.transform.rotation + " firepoint rotation");
        
        if (canShoot && currentAmmo > 0)
        {
            
            audioSource.clip = shotSoundClip;
            audioSource.Play();

            int rand = UnityEngine.Random.Range(0, 3);
            muzzleFlash = firePoint.GetChild(0).GetChild(rand);
            muzzleFlash.gameObject.SetActive(true);
            Animator muzzle = firePoint.GetChild(0).GetComponent<Animator>();
            muzzle.SetTrigger("Shoot");

            if (isADS)
            {
                spr = new Vector3(0, 0, adsSpread);
            }
            else
            {
                spr = new Vector3(0, 0, hipSpread);
            }
            
            firePoint.transform.Rotate(-spr * 3/2);
            for (int i = 0; i < numBullets; i ++)
            {
                var hit = Physics2D.Raycast(
                    firePoint.position,
                    firePoint.transform.up,
                    weaponRange
                );

                /*var trail = Instantiate(
                    bulletTrail,
                    firePoint.position,
                    firePoint.transform.rotation
                );*/
                var trail = PhotonNetwork.Instantiate("BulletTrail", firePoint.position, firePoint.transform.rotation);

                var trailScript = trail.GetComponent<BulletTrailScript>();

                if (hit.collider != null && hit.collider.GetComponent<ISecondaryGadget>() == null) {
                    trailScript.SetTargetPosition(hit.point);
                    var hittable = hit.collider.GetComponent<IShootAble>();
                    if (hittable != null) {
                        //Debug.Log("shotgun calling recieve hit on player?");
                        hittable.RecieveHit(hit, damage);
                    } else {
                        hittable = hit.collider.transform.parent.gameObject.GetComponent<IShootAble>();
                        if (hittable != null) {
                            hittable.RecieveHit(hit, damage);
                        } else if (hit.collider.transform.parent.parent != null) {
                            hittable = hit.collider.transform.parent.parent.gameObject.GetComponent<IShootAble>();
                            if (hittable != null) {
                                hittable.RecieveHit(hit, damage);
                            } else if (hit.collider.GetComponent<BarricadeScript>()) {
                                var hitBarricade = hit.collider.GetComponent<BarricadeScript>();
                                if (hitBarricade != null) {
                                    hitBarricade.RecieveHit(hit.point, firePoint.transform.position.x, firePoint.transform.position.y);
                                }
                            } else if (hit.collider.GetComponent<SoftWallScript>()) {
                                var hitSoftWall = hit.collider.GetComponent<SoftWallScript>();
                                if (hitSoftWall != null) {
                                    hitSoftWall.RecieveHitRaycast(hit, firePoint.transform.position.x, firePoint.transform.position.y);
                                }
                            }
                        } else if (hit.collider.GetComponent<BarricadeScript>()) {
                            var hitBarricade = hit.collider.GetComponent<BarricadeScript>();
                            if (hitBarricade != null) {
                                hitBarricade.RecieveHit(hit.point, firePoint.transform.position.x, firePoint.transform.position.y);
                            }
                        } else if (hit.collider.GetComponent<SoftWallScript>()) {
                            var hitSoftWall = hit.collider.GetComponent<SoftWallScript>();
                            if (hitSoftWall != null) {
                                hitSoftWall.RecieveHitRaycast(hit, firePoint.transform.position.x, firePoint.transform.position.y);
                            }
                        }
                    }
                } else {
                    var endPosition = firePoint.position + firePoint.up * weaponRange;
                    trailScript.SetTargetPosition(endPosition);
                }

                // get ready for next bullet
                firePoint.transform.Rotate(spr);
            }

            canShoot = false;
            currentAmmo --;
            //Debug.Log("current ammo = " + currentAmmo);
            Invoke("ShootingHelper", timeBetweenShots);
            firePoint.transform.Rotate(-spr * 5/2);

            return true;
        }
        return false;
    }
    private void ShootingHelper()
    {
        canShoot = true;
        muzzleFlash.gameObject.SetActive(false);
    }

    public void reload(AudioSource audioSource, PlayerMovement pm)
    {
        //Debug.Log("reload called " + reloading);
        if (!reloading)
        {
            playerMovement = pm;
            Invoke(nameof(reloadHelper), reloadTime);
            reloading = true;
            audioSource.clip = reloadSoundClip;
            audioSource.Play();
        }
    }
    public void reloadHelper()
    {
        //Debug.Log("reload helper called");
        currentAmmo = maxAmmo;
        reloading = false;
        playerMovement.ammoHUD.setCurrentAmmo(maxAmmo);
    }
    public void aimDownSight(Light2D light2D)
    {
        light2D.pointLightInnerAngle = adsInnerAngle;
        light2D.pointLightOuterAngle = adsOuterAngle;
        weaponRange += adsRangeIncrease;
        isADS = true;
    }
    public void hipFire(Light2D light2D, float innerAngle, float outerAngle)
    {
        light2D.pointLightInnerAngle = innerAngle;
        light2D.pointLightOuterAngle = outerAngle;
        weaponRange -= adsRangeIncrease;
        isADS = false;
    }
    public Sprite getSprite() {
        return sprite;
    }
    public int getMaxAmmo() {
        return maxAmmo;
    }
}
