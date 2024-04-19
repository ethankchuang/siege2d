using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;

public class Shooting : MonoBehaviour
{
    [SerializeField] public Transform firePoint;
    //[SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject bulletTrail;
    [SerializeField] private float weaponRange = 100f;
    [SerializeField] private Animator muzzleFlashAnimator;
    [SerializeField] private float spread;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float reloadTime;


    PhotonView view;
    bool firing = false;
    bool reloading = false;
    bool canShoot = true;
    float spr;


    //public float bulletForce = 20f;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // shooting
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
                spr = Random.Range(-spread, spread);

                var hit = Physics2D.Raycast(
                    firePoint.position,
                    transform.up,
                    weaponRange
                );

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
                        hittable.RecieveHit(hit);
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
                Debug.Log("current ammo = " + currentAmmo);
                Invoke("ShootingHelper", timeBetweenShots);
            }
        }
    }

    private void ShootingHelper()
    {
        canShoot = true;
    }
}
