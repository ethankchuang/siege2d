using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AssaultRifleScript : MonoBehaviour, IWeaponScript  
{
    [SerializeField] public GameObject bulletTrail;
    [SerializeField]private float weaponRange;
    [SerializeField]private float hipSpread;
    [SerializeField]private float adsSpread;
    [SerializeField]private float maxAmmo;
    private float currentAmmo;
    [SerializeField]private float timeBetweenShots;
    [SerializeField]private float reloadTime;
    [SerializeField]private int adsInnerAngle;
    [SerializeField]private int adsOuterAngle;
    [SerializeField]private int damage;
    [SerializeField]private AudioClip shotSoundClip;
    [SerializeField]private AudioClip reloadSoundClip;
    private Transform muzzleFlash;
    bool firing = false;
    bool reloading = false;
    bool canShoot = true;
    bool isADS = false;
    Vector3 spr;

    public void onStart()
    {
        currentAmmo = maxAmmo;
        Debug.Log("on start called " + currentAmmo);
    }
    public void shoot(Transform firePoint, AudioSource audioSource)
    {
        //Debug.Log("shoot called " + canShoot + " ca n shoot " + currentAmmo + " current ammo");
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
                spr = new Vector3(0, 0, Random.Range(-adsSpread, adsSpread));
            }
            else
            {
                spr = new Vector3(0, 0, Random.Range(-hipSpread, hipSpread));
            }

            firePoint.transform.Rotate(spr);

            var hit = Physics2D.Raycast(
                firePoint.position,
                firePoint.transform.up,
                weaponRange
            );

            firePoint.transform.Rotate(-spr);

            var trail = Instantiate(
                bulletTrail,
                firePoint.position,
                firePoint.transform.rotation
            );

            var trailScript = trail.GetComponent<BulletTrailScript>();

            if (hit.collider != null) {
                trailScript.SetTargetPosition(hit.point);
                var hittable = hit.collider.GetComponent<IShootAble>();
                if (hittable != null) {
                    hittable.RecieveHit(hit, damage);
                } else {
                    hittable = hit.collider.transform.parent.GetComponent<IShootAble>();
                    if (hittable != null) {
                        hittable.RecieveHit(hit, damage);
                    } else {
                        hittable = hit.collider.transform.parent.parent.GetComponent<IShootAble>();
                        if (hittable != null) {
                            hittable.RecieveHit(hit, damage);
                        } else if (hit.collider.GetComponent<BarricadeScript>() != null) {
                            var hitBarricade = hit.collider.GetComponent<BarricadeScript>();
                            if (hitBarricade != null) {
                                hitBarricade.RecieveHit(hit.point, firePoint.transform.position.x, firePoint.transform.position.y);
                            }
                        }
                    }
                }
            } else {
                var endPosition = firePoint.position + firePoint.transform.up * weaponRange;
                trailScript.SetTargetPosition(endPosition);
            }
            canShoot = false;
            currentAmmo --;
            //Debug.Log("current ammo = " + currentAmmo);
            Invoke("ShootingHelper", timeBetweenShots);
        }
    }
    private void ShootingHelper()
    {
        canShoot = true;
        muzzleFlash.gameObject.SetActive(false);
    }

    public void reload(AudioSource audioSource)
    {
        if (!reloading)
        {
            Invoke(nameof(reloadHelper), reloadTime);
            reloading = true;
            audioSource.clip = reloadSoundClip;
            audioSource.Play();
        }
    }

    public void reloadHelper()
    {
        currentAmmo = maxAmmo;
        reloading = false;
    }
    public void aimDownSight(Light2D light2D)
    {
        light2D.pointLightInnerAngle = adsInnerAngle;
        light2D.pointLightOuterAngle = adsOuterAngle;
        isADS = true;
    }

    public void hipFire(Light2D light2D, float innerAngle, float outerAngle)
    {
        light2D.pointLightInnerAngle = innerAngle;
        light2D.pointLightOuterAngle = outerAngle;
        isADS = false;
    }
}
