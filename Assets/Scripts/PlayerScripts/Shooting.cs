using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class Shooting : MonoBehaviour
{
    [SerializeField] public Transform firePoint;
    //[SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject bulletTrail;
    [SerializeField] private float weaponRange = 100f;
    [SerializeField] private Animator muzzleFlashAnimator;
    [SerializeField] private float hipSpread;
    [SerializeField] private float adsSpread;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float reloadTime;

    [SerializeField] private int innerAngle;
    [SerializeField] private int outerAngle;
    [SerializeField] private int adsInnerAngle;
    [SerializeField] private int adsOuterAngle;

    [SerializeField] private int damage;


    PhotonView view;
    bool firing = false;
    bool reloading = false;
    bool canShoot = true;
    bool isADS = false;
    Vector3 spr;

    [SerializeField] Light2D light2D;

    //public float bulletForce = 20f;
    void Start()
    {
        view = GetComponent<PhotonView>();
        light2D.pointLightInnerAngle = innerAngle;
        light2D.pointLightOuterAngle = outerAngle;
    }

    // Update is called once per frame
    void Update()
    {
        // shooting

        // USE GET BUTTON INSTEAD OF GET BUTTON DOWN??
        if (Input.GetButtonDown("Fire1") || firing)
        {
            firing = true;
            Shoot();
            if (Input.GetButtonUp("Fire1"))
            {
                firing = false;
            }
        }

        // reloading
        if (Input.GetKeyDown("r") && !reloading) Invoke("Reload", reloadTime);

        // block cam when ads
        if (Input.GetButton("Fire2"))
        {
            light2D.pointLightInnerAngle = adsInnerAngle;
            light2D.pointLightOuterAngle = adsOuterAngle;
            isADS = true;
        }
        else if (isADS && Input.GetButtonUp("Fire2"))
        {
            light2D.pointLightInnerAngle = innerAngle;
            light2D.pointLightOuterAngle = outerAngle;
            isADS = false;
        }

    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("finished reloading");
    }
    private void Shoot()
    {
        if (view.IsMine)
        {
            if (canShoot && currentAmmo > 0)
            {
                if (isADS)
                {
                    spr = new Vector3(0, 0, Random.Range(-adsSpread, adsSpread));
                }
                else
                {
                    spr = new Vector3(0, 0, Random.Range(-hipSpread, hipSpread));
                }

                transform.Rotate(spr);

                var hit = Physics2D.Raycast(
                    firePoint.position,
                    transform.up,
                    weaponRange
                );

                transform.Rotate(-spr);

                var trail = Instantiate(
                    bulletTrail,
                    firePoint.position,
                    transform.rotation
                );

                var trailScript = trail.GetComponent<BulletTrailScript>();

                if (hit.collider != null)
                {
                    trailScript.SetTargetPosition(hit.point);
                    var hittable = hit.collider.GetComponent<IShootAble>();
                    if (hittable != null)
                    {
                        hittable.RecieveHit(hit, damage);
                    }
                    else
                    {
                        var hitBarricade = hit.collider.GetComponent<BarricadeScript>();
                        if (hitBarricade != null)
                        {
                            hitBarricade.RecieveHit(hit.point, transform.position.x, transform.position.y);
                        }
                    }
                }
                else
                {
                    var endPosition = firePoint.position + transform.up * weaponRange;
                    trailScript.SetTargetPosition(endPosition);
                }
                canShoot = false;
                currentAmmo --;
                //Debug.Log("current ammo = " + currentAmmo);
                Invoke("ShootingHelper", timeBetweenShots);
            }
        }
    }

    private void ShootingHelper()
    {
        canShoot = true;
    }
}
